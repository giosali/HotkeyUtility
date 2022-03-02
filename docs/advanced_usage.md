# Advanced Usage

This page goes more in depth on some of the features and inner workings of the **HotkeyUtility** package.

## Binding

One of the more convenient features of **HotkeyUtility** is that its objects and controls support binding to its properties.

### The `Combination` Property

Both `HotkeyBinding` and `VisualHotkey` contain a dependency property called `Combination`, which is a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding). This means that if you bind a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) to the `Combination` property, the hotkey(s) in your application can be changed at runtime.

!!! info
    The `Combination` property is really just a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) in disguise. This provides a significant advantage over the [Gesture](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding.gesture#system-windows-input-keybinding-gesture) property of a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) because the Gesture property doesn't allow certain combinations like ++shift+m++ or ++shift+1++.

## Using `HotkeyUtility` Programmatically

So far, you've really only seen **HotkeyUtility** used in XAML but what if you're more of a codebehind kind of person? Well, don't worry because you can simply use the `HotkeyUtility` class to add, remove, or replace hotkeys.

The [HotkeyUtility](https://github.com/giosali/HotkeyUtility/blob/main/HotkeyUtility/HotkeyUtility.cs) class exposes the following methods that you can use:

* `GetHotkeyUtility`
* `TryAddHotkey`
* `TryRemoveHotkey`
* `ReplaceHotkey`
* `GetHotkeys`

### Getting Started

In order to get an instance of the `HotkeyUtility` class, you need to use its `GetHotkeyUtility` method. The `HotkeyUtility` class uses the [singleton design pattern](https://en.wikipedia.org/wiki/Singleton_pattern) which means that as long as you access your `HotkeyUtility` from the *same thread* that created it in the first place, you'll always have access to your hotkeys.

Here's an example where we create an instance of `HotkeyUtility` for the first time in our application:

```csharp linenums="1" title="ShellViewModel.cs"
using HotkeyUtility;

public void InstantiateHotKeyUtility()
{
    HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
}
```

Now, let's say that we've added some hotkeys to our utility (which we'll get to later just shortly in another section) and we want to interact with them in another file. Well, we can do exactly that by calling the same method:

```csharp linenums="1" title="OtherViewModel.cs"
using HotkeyUtility;

public void InstantiateHotKeyUtility()
{
    // This is the same instance as the one in the previous file
    HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
}
```

### Adding Hotkeys

If you want to programmatically add a hotkey to your application, you'll need to:

1. Create a `Hotkey` object
2. Use the `TryAddHotkey` method to add it

`Hotkey` objects have the following constructor signature:

```csharp linenums="1"
public Hotkey(Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler, ushort id = default)
```

You'll need to pass it a [Key](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key), a [ModifierKeys](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.modifierkeys), and an event handler that will be subscribed to the `Pressed` event that will trigger when the hotkey is pressed.

Once you've created your `Hotkey`, pass it to the `TryAddHotkey` method of an instance of the `HotkeyUtility` class:

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    Key key = Key.A;
    ModifierKeys modifiers = ModifierKeys.Shift | ModifierKeys.Control;
    
    Hotkey hotkey = new(key, modifiers, Hotkey_Pressed);
    HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
    _ = hotkeyUtility.TryAddHotkey(hotkey);
}

public static void Hotkey_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine();
}
```

The `TryAddHotkey` method returns a bool indicating whether or not the operation was successful.

### Removing Hotkeys

If you want to remove a hotkey from your application, you can do so by passing the same `Hotkey` object to the `TryRemoveHotkey` method of `HotkeyUtility`:

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    Key key = Key.A;
    ModifierKeys modifiers = ModifierKeys.Shift | ModifierKeys.Control;
    
    Hotkey hotkey = new(key, modifiers, Hotkey_Pressed);
    HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
    _ = hotkeyUtility.TryAddHotkey(hotkey);

    _ = hotkeyUtility.TryRemoveHotkey(hotkey);
}

public static void Hotkey_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine();
}
```

Now, ++ctrl+shift+a++ will be unregistered from your application.

The `TryRemoveHotkey` method returns a bool indicating whether or not the operation was successful.

### Replacing Hotkeys

The `ReplaceHotkey` method works by replacing the [Key](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key) and [ModifierKeys](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.modifierkeys) for an existing `Hotkey` by passing its `Id` property:

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
    Hotkey hotkey = new(Key.A, ModifierKeys.Shift | ModifierKeys.Control, Hotkey_Pressed);
    _ = hotkeyUtility.TryAddHotkey(hotkey);

    // Now, pressing Shift + Control + A is no longer registered and Hotkey_Pressed
    // will no longer be triggered by that hotkey.
    // Instead, if the user presses Alt + B, Hotkey_Pressed will be triggered.
    hotkeyUtility.ReplaceHotkey(hotkey.Id, Key.B, ModifierKeys.Alt);
}

public static void Hotkey_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine();
}
```

### Viewing Your Hotkeys

You can iterate over all of your current hotkeys by calling the `GetHotkeys` method. This will return a [ValueCollection](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2.valuecollection) of `Hotkey` objects that you can then iterate over:

```csharp linenums="1"
using System;
using HotkeyUtility;

public static void Main()
{
    HotkeyUtility hotkeyUtility = HotkeyUtility.GetHotkeyUtility();
    foreach (Hotkey hotkey in hotkeyUtility.GetHotkeys())
    {
        // Do something here
    }
}
```

