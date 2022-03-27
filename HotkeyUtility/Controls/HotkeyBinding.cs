using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using HotkeyUtility.Input;

namespace HotkeyUtility.Controls
{
    /// <summary>
    /// Binds a <see cref="KeyBinding"/> to an <see cref="EventHandler"/> and/or to a <see cref="RoutedCommand"/> (or another <see cref="ICommand"/> implementation).
    /// </summary>
    public class HotkeyBinding : KeyBinding
    {
        /// <summary>
        /// Identifies the <see cref="Combination"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CombinationProperty = DependencyProperty.RegisterAttached(
            "Combination",
            typeof(KeyBinding),
            typeof(HotkeyBinding),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: CombinationPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyBinding"/> class.
        /// </summary>
        public HotkeyBinding()
        {
            Pressed += OnPressed;
        }

        /// <summary>
        /// Occurs when a <see cref="Combination"/> is triggered.
        /// </summary>
        public event EventHandler<HotkeyEventArgs> Pressed;

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
        /// Gets or sets the ID of this <see cref="HotkeyBinding"/>.
        /// </summary>
        private ushort Id { get; set; }

        /// <summary>
        /// Is invoked in response to a changing dependency property of type <see cref="Combination"/>.
        /// </summary>
        /// <param name="d">The previous value of the data member.</param>
        /// <param name="e">Event data that contains information about which property changed, and its old and new values.</param>
        private static void CombinationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HotkeyBinding hotkeyBinding = (HotkeyBinding)d;
            Hotkey hotkey = new(hotkeyBinding.Combination.Key, hotkeyBinding.Combination.Modifiers, hotkeyBinding.Pressed, hotkeyBinding.Id);

            // The HotkeyBinding's Id property will only change if it is 0.
            hotkeyBinding.Id = hotkey.Id;

            // Typically, the reason a Hotkey won't be able to be added is because
            // its Id has already been registered (meaning this HotkeyBinding is
            // currently active). If an active HotkeyBinding's Combination property changes,
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
        /// <param name="sender"></param>
        /// <param name="e">Event data that contains information about the invoked hotkey.</param>
        private void OnPressed(object sender, HotkeyEventArgs e)
        {
            Command?.Execute(CommandParameter);
        }
    }
}
