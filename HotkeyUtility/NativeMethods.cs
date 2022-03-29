﻿using System;
using System.Runtime.InteropServices;

namespace HotkeyUtility
{
    public static class NativeMethods
    {
        /// <summary>
        /// Posted when the user presses a hotkey registered by the RegisterHotKey function.
        /// </summary>
        internal const int WM_HOTKEY = 0x0312;

        /// <summary>
        /// Hotkey is already registered.
        /// </summary>
        internal const int ERROR_HOTKEY_ALREADY_REGISTERED = 0x00000581;

        /// <summary>
        /// Defines a system-wide hotkey.
        /// </summary>
        /// <param name="hWnd">A handle to the window that will receive WM_HOTKEY messages generated by the hotkey. If this parameter is NULL, WM_HOTKEY messages are posted to the message queue of the calling thread and must be processed in the message loop.</param>
        /// <param name="id">The identifier of the hotkey. If the hWnd parameter is NULL, then the hotkey is associated with the current thread rather than with a particular window. If a hotkey already exists with the same hWnd and id parameters, see Remarks for the action taken.</param>
        /// <param name="fsModifiers">The keys that must be pressed in combination with the key specified by the uVirtKey parameter in order to generate the WM_HOTKEY message. The fsModifiers parameter can be a combination of the following values.</param>
        /// <param name="vk">The virtual-key code of the hotkey.</param>
        /// <returns><see langword="true"/> if the method succeeds; otherwise, <see langword="false"/>.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        /// <summary>
        /// Frees a hotkey previously registered by the calling thread.
        /// </summary>
        /// <param name="hWnd">A handle to the window associated with the hotkey to be freed. This parameter should be NULL if the hotkey is not associated with a window.</param>
        /// <param name="id">The identifier of the hotkey to be freed.</param>
        /// <returns><see langword="true"/> if the method succeeds; otherwise, <see langword="false"/>.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
