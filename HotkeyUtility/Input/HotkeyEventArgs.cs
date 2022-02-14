using System.Windows;

namespace HotkeyUtility.Input
{
    public class HotkeyEventArgs : RoutedEventArgs
    {
        public HotkeyEventArgs()
        {
        }

        public HotkeyEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {
        }
    }
}
