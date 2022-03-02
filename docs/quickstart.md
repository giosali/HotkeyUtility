# Quickstart

**HotkeyUtility** is designed and intended to be intuitive and easy to use. This page will show a couple of quick examples to get you started using hotkeys in your WPF applications.

## Creating a Hotkey

Setting up a hotkey in your application is easy. We first need to declare the **HotkeyUtility** XAML namespace in the root tag of your XAML file:

```xml linenums="1" title="MainWindow.xaml"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"

        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility">
</Window>
```

Once the namespace is declared, you can use the `HotkeyBinding` object (which inherits from [InputBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding)) that's provided to you by **HotkeyUtility**:

Simply add it to the [InputBindings](https://docs.microsoft.com/en-us/dotnet/api/system.windows.uielement.inputbindings) property of the [Window](https://docs.microsoft.com/en-us/dotnet/api/system.windows.window):

```xml linenums="1" title="MainWindow.xaml"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility">
    <Window.InputBindings>
        <hu:HotkeyBinding Combination="Alt + Space"
                          Pressed="HotkeyBinding_Pressed"/>
    </Window.InputBindings>
</Window>
```

Once that's set up, your hotkey is now registered and ready to be used. Whenever the user simultaneously presses ++alt+space++, the event handler called ***HotkeyBinding_Pressed*** in the codebehind will be triggered:

```csharp linenums="1" title="MainWindow.xaml.cs"
using System;
using HotkeyUtility.Input;

public void HotkeyBinding_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine("A hotkey was pressed");
}
```

## Hotkeys and MVVM

If you're using an MVVM framework (like [Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro), for example), there's a chance that attaching an event handler to the `Pressed` event of `HotkeyBinding` won't work.

If that's the case for you, you'll need to use the `VisualHotkey` control provided by **HotkeyUtility**. It's important to point out that the name is a bit of a misnomer; `VisualHotkey` has no visual properties so it won't take up any space in your application.

You can use it in your XML file like in this example (which uses [Caliburn.Micro](https://github.com/Caliburn-Micro/Caliburn.Micro)):

```xml linenums="1" title="ShellView.xaml"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility"
        xmlns:cal="http://www.caliburnproject.org">
    <Grid>
        <hu:VisualHotkey Combination="Alt + Space"
                         cal:Message.Attach="[Event Pressed] = [Action VisualHotkey_Pressed($this, $eventArgs)]"/>
    </Grid>
</Window>
```

!!! info
    One possible reason why it might not work is because [InputBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) doesn't inherit from [FrameworkElement](https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement).

## You Can Still Use Commands

If you're more comfortable using [ICommand](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.icommand) and you'd rather use a command than an event handler, that's also available as an option for both the `HotkeyBinding` and the `VisualHotkey`:

```xml linenums="1" title="ShellView.xaml"
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility">
    <Window.InputBindings>
        <hu:HotkeyBinding Combination="Alt + Space"
                          Command="{Binding MyCommand}"/>
    </Window.InputBindings>

    <Grid>
        <hu:VisualHotkey Combination="Shift + M"
                         Command="{Binding MyOtherCommand}"/>
    </Grid>
</Window>
```