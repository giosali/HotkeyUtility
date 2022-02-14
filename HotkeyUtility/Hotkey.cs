using System;
using System.Windows.Input;
using HotkeyUtility.Input;

namespace HotkeyUtility
{
    public class Hotkey
    {
        public Hotkey(Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler, ushort id = default)
        {
            Key = key;
            Modifiers = modifiers;
            Pressed += handler;
            Id = id == default ? ++UniqueId : id;
        }

        public event EventHandler<HotkeyEventArgs> Pressed;

        public ushort Id { get; set; }

        public Key Key { get; set; }

        public ModifierKeys Modifiers { get; set; }

        private static ushort UniqueId { get; set; }

        public void OnPressed(HotkeyEventArgs e)
        {
            EventHandler<HotkeyEventArgs> handler = Pressed;
            handler?.Invoke(this, e);
        }
    }
}
