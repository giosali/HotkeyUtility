using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using HotkeyUtility.Input;

namespace HotkeyUtility.Controls
{
    /// <summary>
    /// Represents a global keybinding, which reacts to the <see cref="Pressed"/> event.
    /// </summary>
    public class VisualHotkey : FrameworkElement
    {
        /// <summary>
        /// Identifies the <see cref="Pressed"/> routed event.
        /// </summary>
        public static readonly RoutedEvent PressedEvent = EventManager.RegisterRoutedEvent(
            name: "Pressed",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(HotkeyEventHandler),
            ownerType: typeof(VisualHotkey));

        /// <summary>
        /// Identifies the <see cref="Combination"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CombinationProperty = DependencyProperty.RegisterAttached(
            name: "Combination",
            propertyType: typeof(KeyBinding),
            ownerType: typeof(VisualHotkey),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: CombinationPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            name: "Command",
            propertyType: typeof(ICommand),
            ownerType: typeof(VisualHotkey),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            name: "CommandParameter",
            propertyType: typeof(object),
            ownerType: typeof(VisualHotkey));

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualHotkey"/> class.
        /// </summary>
        public VisualHotkey()
        {
        }

        /// <summary>
        /// Occurs when a <see cref="Combination"/> is triggered.
        /// </summary>
        public event HotkeyEventHandler Pressed
        {
            add { AddHandler(PressedEvent, value); }
            remove { RemoveHandler(PressedEvent, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Combination"/>.
        /// </summary>
        [TypeConverter(typeof(KeyBindingConverter))]
        public KeyBinding Combination
        {
            get => (KeyBinding)GetValue(CombinationProperty);
            set => SetValue(CombinationProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="Command"/>.
        /// </summary>
        [TypeConverter(typeof(CommandConverter))]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandParameter"/>.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the ID of this <see cref="VisualHotkey"/>.
        /// </summary>
        private ushort Id { get; set; }

        /// <summary>
        /// Is invoked in response to a changing dependency property of type <see cref="Combination"/>.
        /// </summary>
        /// <param name="d">The previous value of the data member.</param>
        /// <param name="e">Event data that contains information about which property changed, and its old and new values.</param>
        public static void CombinationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualHotkey visualHotkey = (VisualHotkey)d;
            Hotkey hotkey = new(visualHotkey.Combination.Key, visualHotkey.Combination.Modifiers, visualHotkey.OnPressed, visualHotkey.Id);

            // The VisualHotkey's Id property will only change if it is 0.
            visualHotkey.Id = hotkey.Id;

            // Typically, the reason a Hotkey won't be able to be added is because
            // its Id has already been registered (meaning this VisualHotkey is
            // currently active). If an active VisualHotkey's Combination property changes,
            // then the current key binding needs to be replaced with a new key binding.
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            if (!hotkeyManager.TryAddHotkey(hotkey))
            {
                _ = hotkeyManager.TryReplaceHotkey(hotkey.Id, hotkey.Key, hotkey.Modifiers);
            }
        }

        /// <summary>
        /// An event reporting that the associated <see cref="Combination"/> was triggered.
        /// </summary>
        /// <param name="sender">The object that invoked the event.</param>
        /// <param name="e">vent data that contains information about the invoked hotkey.</param>
        private void OnPressed(object sender, HotkeyEventArgs e)
        {
            HotkeyEventArgs args = new(PressedEvent);
            RaiseEvent(args);
            Command?.Execute(CommandParameter);
        }
    }
}
