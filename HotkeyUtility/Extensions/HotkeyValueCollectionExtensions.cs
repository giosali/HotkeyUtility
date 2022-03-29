using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace HotkeyUtility.Extensions
{
    public static class HotkeyValueCollectionExtensions
    {
        /// <summary>
        /// Searches for a <see cref="Hotkey"/> whose <see cref="Key"/> and <see cref="ModifierKeys"/> match the specified <paramref name="key"/> and <paramref name="modifiers"/>.
        /// </summary>
        /// <param name="collection">A <see cref="Dictionary{ushort, Hotkey}.ValueCollection"/> to return a <see cref="Hotkey"/> from.</param>
        /// <param name="key">A <see cref="Key"/> to match.</param>
        /// <param name="modifiers">A <see cref="ModifierKeys"/> to match.</param>
        /// <returns>A <see cref="Hotkey"/> if a match was found; otherwise, <see langword="null"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="key"/> and <paramref name="modifiers"/> are respectively equal to <see cref="Key.None"/> and <see cref="ModifierKeys.None"/>.</exception>
        public static Hotkey Find(this Dictionary<ushort, Hotkey>.ValueCollection collection, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
        {
            if (key == Key.None && modifiers == ModifierKeys.None)
            {
                throw new ArgumentException($"{key} and {modifiers} cannot both be None");
            }

            return collection.FirstOrDefault(hotkey => hotkey.Key == key && hotkey.Modifiers == modifiers);
        }
    }
}
