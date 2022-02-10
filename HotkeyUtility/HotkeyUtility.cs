using HotkeyUtility.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Interop;

namespace HotkeyUtility
{
    class HotkeyUtility
    {
        private static readonly object _locker = new();

        protected HotkeyUtility()
        {
            if (Handle == IntPtr.Zero)
            {
                HwndSourceParameters parameters = new("HotkeyUtility Window")
                {
                    WindowStyle = 0
                };

                HwndSource source = new(parameters);
                source.AddHook(WndProc);
                Handle = source.Handle;
            }
        }

        private static IntPtr Handle { get; set; }

        private static HotkeyUtility Instance { get; set; }

        private Dictionary<ushort, Hotkey> Keys { get; set; } = new();

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

        public bool TryAddHotkey(Hotkey hotkey)
        {
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

        public bool TryRemoveHotkey(Hotkey hotkey)
        {
            bool success = Keys.Remove(hotkey.Id);
            if (success)
            {
                lock (_locker)
                {
                    UnregisterHotkey(Handle, hotkey.Id);
                }
            }

            return success;
        }

        public void ReplaceHotkey(Hotkey hotkey)
        {
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

        private static void RegisterHotkey(Hotkey hotkey)
        {
            bool registerSuccess = NativeMethods.RegisterHotKey(Handle, hotkey.Id, (uint)hotkey.Modifiers, (uint)KeyInterop.VirtualKeyFromKey(hotkey.Key));
            if (!registerSuccess)
            {
                throw new ApplicationException($"Keystrokes specified for the hotkey (Key: {hotkey.Key} | Modifiers: {hotkey.Modifiers}) have already been registered by another hotkey");
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
