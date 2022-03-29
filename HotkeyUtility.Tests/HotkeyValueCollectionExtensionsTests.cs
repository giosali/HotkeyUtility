using HotkeyUtility.Extensions;
using NUnit.Framework;
using System;
using System.Threading;
using System.Windows.Input;

namespace HotkeyUtility.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class HotkeyValueCollectionExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Find_WhenGivenValidKeyAndModifierKeys_ShouldReturnHotkey()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);
            Assert.IsInstanceOf<Hotkey>(hotkeyManager.GetHotkeys().Find(Key.Space, ModifierKeys.Alt));

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        public void Find_WhenGivenInvalidKeyAndModifierKeys_ShouldReturnNull()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, null);
            _ = hotkeyManager.TryAddHotkey(hotkey);
            Assert.IsNull(hotkeyManager.GetHotkeys().Find(Key.D, ModifierKeys.Control));

            _ = hotkeyManager.TryRemoveHotkey(hotkey);
        }

        [Test]
        public void Find_WhenGivenKeyNoneAndModifierKeysNone_ShouldThrowArgumentException()
        {
            HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
            Assert.Throws<ArgumentException>(() => hotkeyManager.GetHotkeys().Find(Key.None, ModifierKeys.None));
        }
    }
}
