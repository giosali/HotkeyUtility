using HotkeyUtility.Input;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace HotkeyUtility.Controls
{
    public class HotkeyBinding : KeyBinding
    {
        public static readonly DependencyProperty CombinationProperty = DependencyProperty.RegisterAttached(
            "Combination",
            typeof(KeyBinding),
            typeof(HotkeyBinding),
            new PropertyMetadata(defaultValue: null, propertyChangedCallback: CombinationPropertyChanged));

        public HotkeyBinding()
        {
            Pressed += OnPressed;
        }

        public event EventHandler<HotkeyEventArgs> Pressed;

        [TypeConverter(typeof(KeyBindingConverter))]
        public KeyBinding Combination
        {
            get => (KeyBinding)GetValue(CombinationProperty);
            set => SetValue(CombinationProperty, value);
        }

        private ushort Id { get; set; }

        private static void CombinationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HotkeyBinding hotkeyBinding = (HotkeyBinding)d;
            KeyBinding combination = hotkeyBinding.Combination;
            Hotkey hotkey = new(combination.Key, combination.Modifiers, hotkeyBinding.Pressed, hotkeyBinding.Id);
            hotkeyBinding.Id = hotkey.Id;
            HotkeyUtility utility = HotkeyUtility.GetHotkeyUtility();
            if (!utility.TryAddHotkey(hotkey))
            {
                utility.ReplaceHotkey(hotkey);
            }
        }

        private void OnPressed(object sender, HotkeyEventArgs e)
        {
            Command?.Execute(CommandParameter);
        }
    }
}
