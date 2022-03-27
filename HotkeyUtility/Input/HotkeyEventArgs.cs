using System.Windows;

namespace HotkeyUtility.Input
{
    /// <summary>
    /// Contains state information and event data associated with a hotkey-pressed event.
    /// </summary>
    public class HotkeyEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyEventArgs"/> class.
        /// </summary>
        public HotkeyEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyEventArgs"/> class, using the supplied routed event identifier.
        /// </summary>
        /// <param name="routedEvent">The routed event identifier for this instance of the <see cref="HotkeyEventArgs"/> class.</param>
        public HotkeyEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }
    }
}
