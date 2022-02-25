using System;
using System.Windows.Input;
using Xunit;

namespace HotkeyUtility.Tests
{
    public static class HotkeyUtilityTests
    {
        [WpfFact]
        public static void GetHotkeyUtility_ShouldReturnSameHotkeyUtilityInstance()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            _ = hotkeyUtility.TryAddHotkey(new Hotkey(Key.Space, ModifierKeys.Alt, null));

            HotkeyUtility anotherHotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            int count = 0;
            foreach (Hotkey hotkey in anotherHotkeyUtility.GetHotkeys())
            {
                count++;
            }

            Assert.True(hotkeyUtility == anotherHotkeyUtility);
            Assert.True(count == 1);
        }

        [WpfFact]
        public static void TryAddHotkey_WhenGivenValidHotkey_ShouldReturnTrue()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Control, null);
            bool success = hotkeyUtility.TryAddHotkey(hotkey);
            Assert.True(success);

            bool foundHotkey = false;
            foreach (Hotkey h in hotkeyUtility.GetHotkeys())
            {
                if (h.Key == hotkey.Key && h.Modifiers == hotkey.Modifiers)
                {
                    foundHotkey = true;
                    break;
                }
            }

            Assert.True(foundHotkey);
        }

        [WpfFact]
        public static void TryAddHotkey_WhenHotkeyIsAlreadyRegistered_ShouldReturnFalse()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            bool success = hotkeyUtility.TryAddHotkey(hotkey);
            Assert.False(success);
        }
        
        [WpfFact]
        public static void TryAddHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentNullException>(() => hotkeyUtility.TryAddHotkey(null));
        }

        [WpfFact]
        public static void TryRemoveHotkey_WhenGivenValidHotkey_ShouldReturnTrue()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            bool success = hotkeyUtility.TryRemoveHotkey(hotkey);
            Assert.True(success);

            int count = 0;
            foreach (Hotkey h in hotkeyUtility.GetHotkeys())
            {
                count++;
            }

            Assert.Equal(0, count);
        }

        [WpfFact]
        public static void TryRemoveHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentNullException>(() => hotkeyUtility.TryRemoveHotkey(null));
        }

        [WpfFact]
        public static void ReplaceHotkey_WhenGivenValidHotkey_ShouldOnlyReplaceHotkeys()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            hotkey.Modifiers = ModifierKeys.Control;
            hotkeyUtility.ReplaceHotkey(hotkey);

            int count = 0;
            bool hasSameId = false;
            foreach (Hotkey h in hotkeyUtility.GetHotkeys())
            {
                count++;
                if (h.Id == default(ushort) + 1)
                {
                    hasSameId = true;
                }
            }

            Assert.Equal(1, count);
            Assert.True(hasSameId);
        }

        [WpfFact]
        public static void ReplaceHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentNullException>(() => hotkeyUtility.ReplaceHotkey(null));
        }

        [WpfFact]
        public static void GetHotkeys_ShouldReturnCorrectNumberOfHotkeys()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            _ = hotkeyUtility.TryAddHotkey(new Hotkey(Key.Space, ModifierKeys.Alt, null));
            int count = 0;
            foreach (Hotkey hotkey in hotkeyUtility.GetHotkeys())
            {
                count++;
            }

            Assert.Equal(1, count);
        }
    }
}