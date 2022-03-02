using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Interop;
using HotkeyUtility.Input;

namespace HotkeyUtility
{
    public class HotkeyUtility
    {
        private static readonly object _locker = new();

        protected HotkeyUtility()
        {
            if (Handle == IntPtr.Zero)
            {
                HwndSourceParameters parameters = new("HotkeyUtility Window")
                {
                    WindowStyle = 0,
                };

                HwndSource source = new(parameters);
                source.AddHook(WndProc);
                Handle = source.Handle;
            }
        }

        private static IntPtr Handle { get; set; }

        private static HotkeyUtility Instance { get; set; }

        private Dictionary<ushort, Hotkey> Keys { get; set; } = new();

        /// <summary>
        /// Returns a single, global instance of the HotkeyUtility.
        /// </summary>
        /// <returns>A single, global instance of the HotkeyUtility.</returns>
        public static HotkeyUtility GetHotkeyUtility()
        {
            if (Instance is null)
            {
                lock (_locker)
                {
                    if (Instance is null)
                    {
                        Instance = new();
                    }
                }
            }

            return Instance;
        }

        /// <summary>
        /// Attempts to add the specified <see cref="Hotkey"/> to a dictionary, using its Id property as the key, and registers it.
        /// </summary>
        /// <param name="hotkey">The <see cref="Hotkey"/> to add.</param>
        /// <returns><see langword="true"/> if <paramref name="hotkey"/> was added to the dictionary successfully; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hotkey"/> is <see langword="null"/>.</exception>
        public bool TryAddHotkey(Hotkey hotkey)
        {
            if (hotkey is null)
            {
                throw new ArgumentNullException(nameof(hotkey));
            }

            bool success;
            lock (_locker)
            {
                if (success = Keys.TryAdd(hotkey.Id, hotkey))
                {
                    RegisterHotkey(hotkey);
                }
            }

            return success;
        }

        /// <summary>
        /// Attempts to remove the specified <see cref="Hotkey"/> from a dictionary and unregisters it.
        /// </summary>
        /// <param name="hotkey">The <see cref="Hotkey"/> to remove.</param>
        /// <returns><see langword="true"/> if <paramref name="hotkey"/> was remove from the dictionary successfully; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hotkey"/> is <see langword="null"/>.</exception>
        public bool TryRemoveHotkey(Hotkey hotkey)
        {
            if (hotkey is null)
            {
                throw new ArgumentNullException(nameof(hotkey));
            }

            bool success = Keys.Remove(hotkey.Id);
            if (success)
            {
                lock (_locker)
                {
                    return UnregisterHotkey(Handle, hotkey.Id);
                }
            }

            return success;
        }

        /// <summary>
        /// Unregisters the existing <paramref name="hotkey"/> and registers its new Key and Modifiers properties.
        /// </summary>
        /// <param name="hotkey">The <see cref="Hotkey"/> to replace.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hotkey"/> is <see langword="null"/>.</exception>
        public void ReplaceHotkey(Hotkey hotkey)
        {
            if (hotkey is null)
            {
                throw new ArgumentNullException(nameof(hotkey));
            }

            ushort id = hotkey.Id;
            if (Keys.ContainsKey(id))
            {
                lock (_locker)
                {
                    if (UnregisterHotkey(Handle, id))
                    {
                        Keys[id] = hotkey;
                        RegisterHotkey(hotkey);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters an existing <see cref="Hotkey"/> by its <paramref name="id"/> and re-registers it with a new <paramref name="key"/> and <paramref name="modifiers"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Hotkey"/> to replace.</param>
        /// <param name="key">The new <see cref="Key"/> of the <see cref="Hotkey"/>.</param>
        /// <param name="modifiers">The new <see cref="ModifierKeys"/> of the <see cref="Hotkey"/>.</param>
        /// <exception cref="ArgumentException"><paramref name="key"/> and <paramref name="modifiers"/> are <see langword="null"/>.</exception>
        public void ReplaceHotkey(ushort id, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
        {
            if (key == Key.None && modifiers == ModifierKeys.None)
            {
                throw new ArgumentException($"{key} and {modifiers} cannot both be None");
            }

            if (Keys.ContainsKey(id))
            {
                lock (_locker)
                {
                    if (UnregisterHotkey(Handle, id))
                    {
                        Hotkey hotkey = Keys[id];
                        hotkey.Key = key;
                        hotkey.Modifiers = modifiers;
                        RegisterHotkey(hotkey);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="Dictionary{TKey, TValue}.ValueCollection"/> of hotkeys.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey, TValue}.ValueCollection"/> of <see cref="Hotkey"/></returns>
        public Dictionary<ushort, Hotkey>.ValueCollection GetHotkeys()
        {
            return Keys.Values;
        }

        private static void RegisterHotkey(Hotkey hotkey)
        {
            bool success = NativeMethods.RegisterHotKey(Handle, hotkey.Id, (uint)hotkey.Modifiers, (uint)KeyInterop.VirtualKeyFromKey(hotkey.Key));
            if (!success)
            {
                throw new ApplicationException($"The keystrokes specified for the hotkey (Key: {hotkey.Key} | Modifiers: {hotkey.Modifiers}) have already been registered by another hotkey");
            }
        }

        private static bool UnregisterHotkey(IntPtr hWnd, ushort id)
        {
            return NativeMethods.UnregisterHotKey(hWnd, id);
        }

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
