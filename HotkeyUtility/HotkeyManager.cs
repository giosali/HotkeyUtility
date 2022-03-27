using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using HotkeyUtility.Input;

namespace HotkeyUtility
{
    /// <summary>
    /// Represents a service to manage hotkeys for the current application.
    /// </summary>
    public class HotkeyManager
    {
        /// <summary>
        /// A dedicated object instance.
        /// </summary>
        private static readonly object _lock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyManager"/> class and implements a Win32 window.
        /// </summary>
        protected HotkeyManager()
        {
            if (Handle == IntPtr.Zero)
            {
                HwndSourceParameters parameters = new("HotkeyUtility")
                {
                    WindowStyle = 0,
                };

                HwndSource source = new(parameters);
                source.AddHook(WndProc);
                Handle = source.Handle;
            }
        }

        /// <summary>
        /// Gets or sets the handle to the associated Win32 window of this <see cref="HotkeyManager"/>.
        /// </summary>
        private static IntPtr Handle { get; set; }

        /// <summary>
        /// Gets or sets the single, global instance of <see cref="HotkeyManager"/>.
        /// </summary>
        private static HotkeyManager Instance { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="Hotkey"/> for this <see cref="HotkeyManager"/>.
        /// </summary>
        private Dictionary<ushort, Hotkey> Keys { get; set; } = new();

        /// <summary>
        /// Returns a single, global instance of <see cref="HotkeyManager"/>.
        /// </summary>
        /// <returns>A single, global instance of <see cref="HotkeyManager"/>.</returns>
        public static HotkeyManager GetHotkeyManager()
        {
            if (Instance == null)
            {
                lock (_lock)
                {
                    if (Instance == null)
                    {
                        Instance = new();
                    }
                }
            }

            return Instance;
        }

        /// <summary>
        /// Attempts to add a <see cref="Hotkey"/> to a dictionary and register it.
        /// </summary>
        /// <param name="hotkey">The <see cref="Hotkey"/> to add and register.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Hotkey"/> was successfully added and registered; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hotkey"/> is <see langword="null"/>.</exception>
        public bool TryAddHotkey(Hotkey hotkey)
        {
            if (hotkey is null)
            {
                throw new ArgumentNullException(nameof(hotkey));
            }

            return Keys.TryAdd(hotkey.Id, hotkey) && RegisterHotkey(hotkey);
        }

        /// <summary>
        /// Attempts to remove a <see cref="Hotkey"/> from a dictionary and unregister it.
        /// </summary>
        /// <param name="hotkey">The <see cref="Hotkey"/> to remove and unregister.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Hotkey"/> was successfully removed and unregistered; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hotkey"/> is <see langword="null"/>.</exception>
        public bool TryRemoveHotkey(Hotkey hotkey)
        {
            if (hotkey is null)
            {
                throw new ArgumentNullException(nameof(hotkey));
            }

            return Keys.Remove(hotkey.Id) && UnregisterHotkey(Handle, hotkey.Id);
        }

        /// <summary>
        /// Attempts to locate an existing <see cref="Hotkey"/> through its current key and modifiers, unregister it, and reregister it with a new key and modifiers.
        /// </summary>
        /// <param name="oldKey">The key to match.</param>
        /// <param name="oldModifiers">The modifiers to match.</param>
        /// <param name="newKey">The key to replace the current key.</param>
        /// <param name="newModifiers">The modifiers to replace the current modifiers.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Key"/> and specified <see cref="ModifierKeys"/> were successfully matched and replaced; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="newKey"/> and <paramref name="newModifiers"/> are respectively <see cref="Key.None"/> and <see cref="ModifierKeys.None"/>.</exception>
        public bool TryReplaceHotkey(Key oldKey, ModifierKeys oldModifiers, Key newKey = Key.None, ModifierKeys newModifiers = ModifierKeys.None)
        {
            if (newKey == Key.None && newModifiers == ModifierKeys.None)
            {
                throw new ArgumentException($"{newKey} and {newModifiers} cannot both be None");
            }

            bool success = false;
            foreach (Hotkey hotkey in GetHotkeys())
            {
                if (hotkey.Key == oldKey && hotkey.Modifiers == oldModifiers)
                {
                    if (success = UnregisterHotkey(Handle, hotkey.Id))
                    {
                        hotkey.Key = newKey;
                        hotkey.Modifiers = newModifiers;
                        success = RegisterHotkey(hotkey);
                    }

                    break;
                }
            }

            return success;
        }

        /// <summary>
        /// Attempts to locate an existing <see cref="Hotkey"/> through its Id property, unregister it, and reregister it with a new key and modifiers.
        /// </summary>
        /// <param name="id">The Id of the targeted <see cref="Hotkey"/>.</param>
        /// <param name="newKey">The key to replace the current key.</param>
        /// <param name="newModifiers">The modifiers to replace the current modifiers.</param>
        /// <returns><see langword="true"/> if the specified id was matched and the new key and modifiers were successfully registered; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="id"/> is equal to <see langword="default"/> or <paramref name="newKey"/> and <paramref name="newModifiers"/> are respectively <see cref="Key.None"/> and <see cref="ModifierKeys.None"/>.</exception>
        public bool TryReplaceHotkey(ushort id, Key newKey = Key.None, ModifierKeys newModifiers = ModifierKeys.None)
        {
            if (id == default)
            {
                throw new ArgumentException($"Hotkeys cannot have an ID of {id}", nameof(id));
            }

            if (newKey == Key.None && newModifiers == ModifierKeys.None)
            {
                throw new ArgumentException($"{newKey} and {newModifiers} cannot both be None");
            }

            bool success;
            if (success = Keys.ContainsKey(id))
            {
                if (success = UnregisterHotkey(Handle, id))
                {
                    Hotkey hotkey = Keys[id];
                    hotkey.Key = newKey;
                    hotkey.Modifiers = newModifiers;
                    success = RegisterHotkey(hotkey);
                }
            }

            return success;
        }

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey, TValue}.ValueCollection"/> of hotkeys.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}.ValueCollection"/> of <see cref="Hotkey"/></returns>
        public Dictionary<ushort, Hotkey>.ValueCollection GetHotkeys()
        {
            return Keys.Values;
        }

        /// <summary>
        /// Registers a <see cref="Hotkey"/>.
        /// </summary>
        /// <param name="hotkey">The <see cref="Hotkey"/> to register.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Hotkey"/> was successfully registered; otherwise, <see langword="false"/></returns>
        /// <exception cref="ApplicationException">The specified <see cref="Hotkey"/> is already registered.</exception>
        private static bool RegisterHotkey(Hotkey hotkey)
        {
            bool success = NativeMethods.RegisterHotKey(Handle, hotkey.Id, (uint)hotkey.Modifiers, (uint)KeyInterop.VirtualKeyFromKey(hotkey.Key));
            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                if (error == NativeMethods.ERROR_HOTKEY_ALREADY_REGISTERED)
                {
                    throw new ApplicationException($"The keystrokes specified for the hotkey (Key: {hotkey.Key} | Modifiers: {hotkey.Modifiers}) have already been registered by another hotkey");
                }
            }

            return success;
        }

        /// <summary>
        /// Unregisters a hotkey.
        /// </summary>
        /// <param name="hWnd">The handle to a window.</param>
        /// <param name="id">The Id associated with a <see cref="Hotkey"/>.</param>
        /// <returns><see langword="true"/> if the hotkey was successfully unregistered; otherwise, <see langword="false"/>.</returns>
        private static bool UnregisterHotkey(IntPtr hWnd, ushort id)
        {
            return NativeMethods.UnregisterHotKey(hWnd, id);
        }

        /// <summary>
        /// A callback function that processes hotkey messages associated with the thread that registered the hotkeys. 
        /// </summary>
        /// <param name="hwnd">A handle to the window.</param>
        /// <param name="msg">The message.</param>
        /// <param name="wParam">Additional message information.</param>
        /// <param name="lParam">Additional message information.</param>
        /// <param name="handled">Indicates whether events resulting should be marked handled.</param>
        /// <returns>The return value is the result of the message processing, and depends on the message sent.</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY && Keys.TryGetValue((ushort)wParam.ToInt32(), out Hotkey hotkey))
            {
                hotkey.OnPressed(new HotkeyEventArgs());
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}
