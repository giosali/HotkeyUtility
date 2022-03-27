using NUnit.Framework;
using System;
using System.Threading;
using System.Windows.Input;

namespace HotkeyUtility.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class HotkeyManagerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetHotkeyManager_ShouldReturnSameHotkeyManagerInstance()
        {
            HotkeyManager hm1 = HotkeyManager.GetHotkeyManager();
            HotkeyManager hm2 = HotkeyManager.GetHotkeyManager();
            Assert.True(hm1 == hm2);
        }

        [Test]
        public void TryAddHotkey_WhenGivenValidHotkey_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Control, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenHotkeyIsAlreadyRegistered_ShouldReturnFalse()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Control, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.False(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenOneKey_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.None, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenOneKeyAndOneModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Shift, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenOneKeyAndTwoModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Shift | ModifierKeys.Control, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenOneKeyAndThreeModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenOneModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.None, ModifierKeys.Shift, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenTwoModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.None, ModifierKeys.Shift | ModifierKeys.Control, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenThreeModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.None, ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenNoKeyAndNoModifierKeys_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.None, ModifierKeys.None, null);
            bool success = hotkeyManager.TryAddHotkey(hotkey);
            Assert.True(success);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryAddHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Assert.Throws<ArgumentNullException>(() => hotkeyManager.TryAddHotkey(null));
        }

        [Test]
        public void TryRemoveHotkey_WhenGivenValidHotkey_ShouldReturnTrue()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Control, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);
            bool success = hotkeyManager.TryRemoveHotkey(hotkey);
            Assert.True(success);
        }

        [Test]
        public void TryRemoveHotkey_WhenGivenNull_ShouldThrowArgumentNullException()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Assert.Throws<ArgumentNullException>(() => hotkeyManager.TryRemoveHotkey(null));
        }

        [Test]
        public void TryReplaceHotkey_WhenGivenValidHotkey_ShouldNotChangeHotkeyId()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Control, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);
            _ = hotkeyManager.TryReplaceHotkey(hotkey.Id, hotkey.Key, ModifierKeys.Shift);

            ushort id = default;
            foreach (Hotkey hk in hotkeyManager.GetHotkeys())
            {
                if (hk.Key == hotkey.Key && hk.Modifiers == hotkey.Modifiers)
                {
                    id = hk.Id;
                    break;
                }
            }

            Assert.AreEqual(hotkey.Id, id);

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void TryReplaceHotkey_WhenGivenId_ShouldReplaceHotkey()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Control, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);

            Key key = Key.A;
            ModifierKeys modifiers = ModifierKeys.Shift | ModifierKeys.Control;
            bool success = hotkeyManager.TryReplaceHotkey(hotkey.Id, key, modifiers);
            Assert.True(success);
            Assert.AreEqual(hotkey.Key, key);
            Assert.AreEqual(hotkey.Modifiers, modifiers);
        }

        [Test]
        public void TryReplaceHotkey_WhenGivenNoKeyAndNoModifierKeys_ShouldThrowArgumentException()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Assert.Throws<ArgumentException>(() => hotkeyManager.TryReplaceHotkey(Key.None, ModifierKeys.None, Key.None, ModifierKeys.None));
        }

        [Test]
        public void TryReplaceHotkey_WhenGivenIdButNoKeyAndNoModifierKeys_ShouldThrowArgumentException()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Assert.Throws<ArgumentException>(() => hotkeyManager.TryReplaceHotkey(1, Key.None, ModifierKeys.None));
        }

        [Test]
        public void TryReplaceaHotkey_WhenGivenZeroId_ShouldThrowArgumentException()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Assert.Throws<ArgumentException>(() => hotkeyManager.TryReplaceHotkey(0, Key.Space, ModifierKeys.Control));
        }

        [Test]
        public void GetHotkeys_ShouldReturnCorrectNumberOfHotkeys()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.A, ModifierKeys.Alt, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);
            int count = 0;
            foreach (Hotkey h in hotkeyManager.GetHotkeys())
            {
                count++;
            }

            Assert.AreEqual(1, count);
        }
    }
}