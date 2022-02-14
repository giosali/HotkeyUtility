using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using HotkeyUtility.Input;

namespace HotkeyUtility.Controls
{
    public class VisualHotkey : FrameworkElement
    {
        public static readonly RoutedEvent PressedEvent = EventManager.RegisterRoutedEvent(
            name: "Pressed",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(HotkeyEventHandler),
            ownerType: typeof(VisualHotkey));

        public static readonly DependencyProperty CombinationProperty = DependencyProperty.RegisterAttached(
            name: "Combination",
            propertyType: typeof(KeyBinding),
            ownerType: typeof(VisualHotkey),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: GesturePropertyChanged));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            name: "Command",
            propertyType: typeof(ICommand),
            ownerType: typeof(VisualHotkey),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            name: "CommandParameter",
            propertyType: typeof(object),
            ownerType: typeof(VisualHotkey));

        public VisualHotkey()
        {
        }

        public event HotkeyEventHandler Pressed
        {
            add { AddHandler(PressedEvent, value); }
            remove { RemoveHandler(PressedEvent, value); }
        }

        [TypeConverter(typeof(KeyBindingConverter))]
        public KeyBinding Combination
        {
            get => (KeyBinding)GetValue(CombinationProperty);
            set => SetValue(CombinationProperty, value);
        }

        [TypeConverter(typeof(CommandConverter))]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private ushort Id { get; set; }

        public static void GesturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualHotkey visualHotkey = (VisualHotkey)d;
            KeyBinding combination = visualHotkey.Combination;
            Hotkey hotKey = new(combination.Key, combination.Modifiers, visualHotkey.OnPressed, visualHotkey.Id);
            visualHotkey.Id = hotKey.Id;
            HotkeyUtility utility = HotkeyUtility.GetHotkeyUtility();
            if (!utility.TryAddHotkey(hotKey))
            {
                utility.ReplaceHotkey(hotKey);
            }
        }

        private void OnPressed(object sender, HotkeyEventArgs e)
        {
            HotkeyEventArgs args = new(PressedEvent);
            RaiseEvent(args);
            Command?.Execute(CommandParameter);
        }
    }
}
