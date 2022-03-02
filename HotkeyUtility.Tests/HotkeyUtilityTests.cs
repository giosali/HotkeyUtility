using NUnit.Framework;
using System;
using System.Threading;
using System.Windows.Input;

namespace HotkeyUtility.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class HotkeyUtilityTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetHotkeyUtility_ShouldReturnSameHotkeyUtilityInstance()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            HotkeyUtility anotherHotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.True(hotkeyUtility == anotherHotkeyUtility);
        }

        [Test]
        public void TryAddHotkey_WhenGivenValidHotkey_ShouldReturnTrue()
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

        [Test]
        public void TryAddHotkey_WhenHotkeyIsAlreadyRegistered_ShouldReturnFalse()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Shift, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            bool success = hotkeyUtility.TryAddHotkey(hotkey);
            Assert.False(success);
        }

        [Test]
        public void TryAddHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentNullException>(() => hotkeyUtility.TryAddHotkey(null));
        }

        [Test]
        public void TryRemoveHotkey_WhenGivenValidHotkey_ShouldReturnTrue()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            int initialCount = 0;
            foreach (Hotkey _ in hotkeyUtility.GetHotkeys())
            {
                initialCount++;
            }

            Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            bool success = hotkeyUtility.TryRemoveHotkey(hotkey);
            Assert.True(success);

            int finalCount = 0;
            foreach (Hotkey _ in hotkeyUtility.GetHotkeys())
            {
                finalCount++;
            }

            Assert.AreEqual(initialCount, finalCount);
        }

        [Test]
        public void TryRemoveHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentNullException>(() => hotkeyUtility.TryRemoveHotkey(null));
        }

        [Test]
        public void ReplaceHotkey_WhenGivenValidHotkey_ShouldOnlyReplaceHotkeys()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            int initialCount = 0;
            foreach (Hotkey _ in hotkeyUtility.GetHotkeys())
            {
                initialCount++;
            }

            Hotkey hotkey = new(Key.A, ModifierKeys.Control, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            hotkey.Modifiers = ModifierKeys.Control;
            hotkeyUtility.ReplaceHotkey(hotkey);

            bool hasSameId = false;
            int finalCount = 0;
            foreach (Hotkey h in hotkeyUtility.GetHotkeys())
            {
                finalCount++;
                if (h.Id == hotkey.Id && !hasSameId)
                {
                    hasSameId = true;
                }
            }

            Assert.AreEqual(initialCount + 1, finalCount);
            Assert.True(hasSameId);
        }

        [Test]
        public void ReplaceHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentNullException>(() => hotkeyUtility.ReplaceHotkey(null));
        }

        public void ReplaceHotkey_WhenGivenIdKeyAndModifierKeys_ShouldReplaceHotkey()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.A, ModifierKeys.Shift, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);

            Key key = Key.A;
            ModifierKeys modifiers = ModifierKeys.Shift | ModifierKeys.Control;
            hotkeyUtility.ReplaceHotkey(hotkey.Id, key, modifiers);
            Assert.AreEqual(hotkey.Key, key);
            Assert.AreEqual(hotkey.Modifiers, modifiers);
        }

        [Test]
        public void ReplaceaHotkey_WhenGivenKeyNoneModifierKeysNone_ShouldThrowArgumentException()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Assert.Throws<ArgumentException>(() => hotkeyUtility.ReplaceHotkey(0, Key.None, ModifierKeys.None));
        }

        [Test]
        public void GetHotkeys_ShouldReturnCorrectNumberOfHotkeys()
        {
            HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
            Hotkey hotkey = new(Key.A, ModifierKeys.Alt, null);
            _ = hotkeyUtility.TryAddHotkey(hotkey);
            int count = 0;
            foreach (Hotkey h in hotkeyUtility.GetHotkeys())
            {
                count++;
            }

            Assert.AreEqual(1, count);
        }
    }
}