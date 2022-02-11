<h1 align="center">HotkeyUtility</h1>

<p align="center">
	<img src="https://raw.githubusercontent.com/giosali/HotkeyUtility/main/ext/HotkeyUtility-logo.png" width="150">
</p>

<p align="center">
	A simple library for setting and managing global hotkeys for WPF	
</p>

<p align="center">
    <a href="https://www.nuget.org/packages/HotkeyUtility">
        <img src="https://img.shields.io/nuget/v/HotkeyUtility?logo=nuget" alt="NuGet Version">
    </a>
    <a href="https://github.com/giosali/HotkeyUtility/blob/main/HotkeyUtility/HotkeyUtility.csproj">
        <img src="https://img.shields.io/badge/dynamic/xml?color=%23512bd4&label=target&logo=.net&query=%2F%2FTargetFramework[1]&url=https%3A%2F%2Fraw.githubusercontent.com%2Fgiosali%2FHotkeyUtility%2Fmain%2FHotkeyUtility%2FHotkeyUtility.csproj" alt="Target Framework">
    </a>
</p>

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
  * [XAML](#xaml)
    + [The HotkeyBinding Control](#the-hotkeybinding-control)
    + [Changing the Hotkey at Runtime](#changing-the-hotkey-at-runtime)
  * [Codebehind](#codebehind)
    + [The HotkeyUtility Class](#the-hotkeyutility-class)
    + [Adding a Hotkey](#adding-a-hotkey)
    + [Removing a Hotkey](#removing-a-hotkey)

## Installation

You can find the package on [NuGet](https://www.nuget.org/packages/HotkeyUtility) or install it through PackageManagement:

```ps
Install-Package HotkeyUtility -Version 1.0.0
```

## Usage

There are two ways of using **HotkeyUtility**: through XAML with an MVVM implementation and through the codebehind.

### XAML

#### The HotkeyBinding Control

Declaring the XAML namespace for **HotkeyUtility** will expose the `HotkeyBinding` control that you can place under your `InputBindings`:

```xml
<Window x:Name="window"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility">
    
</Window>
```

The `HotkeyBinding` control exposes the following:

* A dependency property called `Combination` (which is actually a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding?view=windowsdesktop-6.0))
* An event called `Pressed`

Here's a simple example using both in XAML:

```xml
<Window x:Name="window"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility">

    <Window.InputBindings>
        <hu:HotkeyBinding Combination="Alt + Space"
                          Pressed="AltSpace_Pressed"/>
    </Window.InputBindings>

</Window>
```

Now, whenever the user simultaneously presses <kbd>Alt</kbd> and <kbd>Space</kbd>, the event handler *AltSpace_Pressed* will be triggered.

#### Changing the Hotkey at Runtime

Because `Combination` is a dependency property, you can change the system-wide hotkey at runtime:

```xml
<Window x:Name="window"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:hu="clr-namespace:HotkeyUtility.Controls;assembly=HotkeyUtility">

    <Window.InputBindings>
        <hu:HotkeyBinding Combination="{Binding ElementName=window, Path=Binding}"
                          Pressed="AltSpace_Pressed"/>
    </Window.InputBindings>

</Window>
```

Remember that `Combination` is a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding?view=windowsdesktop-6.0), so when you want to programmatically change the hotkey, simply create a new KeyBinding and bind to it:

```cs
KeyBinding keyBinding = new()
{
    Key = Key.Space
    Modifiers = ModifierKeys.Control
};
Binding = keyBinding;
```

```
📝 NOTE: Do not assign a value to the ICommand parameter of the KeyBinding constructor. When the user triggers the HotkeyBinding, the ICommand will be ignored.
```

### Codebehind

#### The HotkeyUtility Class

The `HotkeyUtility` class employs a singleton pattern which means if you declare a `Hotkey` in File1.cs, you will have access to that same `Hotkey` in File2.cs.

#### Adding a Hotkey

If you would like to register a global hotkey programmatically, you can do so by: 

* Invoking the `GetHotkeyUtility` method in the `HotkeyUtility` class
* Instantiating a `Hotkey` object
* And by using the `TryAddHotkey` method of `HotkeyUtility`

```cs
using HotkeyUtility;
using HotkeyUtility.Input;  // Contains HotkeyEventArgs

// EventHandler
public void AltSpace_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine("AltSpace_Pressed called");
}

HotkeyUtility utility = HotkeyUtility.GetHotkeyUtility();
Hotkey hotkey = new(Key.Space, ModifierKeys.Alt, AltSpace_Pressed);
bool success = utility.TryAddHotkey(hotkey);
```

#### Removing a Hotkey

In order to remove a hotkey, use the `TryRemoveHotkey` method on `HotkeyUtility`:

```cs
using HotkeyUtility;

// A Hotkey property
public Hotkey Hotkey { get; set; }

HotkeyUtility utility = HotkeyUtility.GetHotkeyUtility();
bool success = utility.TryRemoveHotkey(Hotkey);
```
