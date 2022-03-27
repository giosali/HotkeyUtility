# Advanced Usage

This page goes more in depth on some of the features and inner workings of the **HotkeyUtility** package.

## Binding

One of the more convenient features of **HotkeyUtility** is that its objects and controls support binding to its properties.

### The `Combination` Property

Both `HotkeyBinding` and `VisualHotkey` contain a dependency property called `Combination`, which is a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding). This means that if you bind a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) to the `Combination` property, the hotkey(s) in your application can be changed at runtime.

!!! info
    The `Combination` property is really just a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) in disguise. This provides a significant advantage over the [Gesture](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding.gesture#system-windows-input-keybinding-gesture) property of a [KeyBinding](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.keybinding) because the Gesture property doesn't allow certain combinations like ++shift+m++ or ++shift+1++.

## Using `HotkeyUtility` Programmatically

So far, you've really only seen **HotkeyUtility** used in XAML but what if you're more of a codebehind kind of person? Well, don't worry because you can simply use the `HotkeyManager` class to add, remove, or replace hotkeys.

The [HotkeyManager](https://github.com/giosali/HotkeyUtility/blob/main/HotkeyUtility/HotkeyManager.cs) class exposes the following methods that you can use:

* `GetHotkeyManager`
* `TryAddHotkey`
* `TryRemoveHotkey`
* `TryReplaceHotkey`
* `GetHotkeys`

### Getting Started

In order to get an instance of the `HotkeyManager` class, you need to use its `GetHotkeyManager` method. The `HotkeyManager` class uses the [singleton design pattern](https://en.wikipedia.org/wiki/Singleton_pattern) which means that as long as you access your `HotkeyManager` from the *same thread* that created it in the first place, you'll always have access to your hotkeys.

Here's a simple example of how we can go about creating an instance of `HotkeyManager` for the first time in our application:

```csharp linenums="1" title="ShellViewModel.cs"
using HotkeyUtility;

public void InstantiateHotkeyManager()
{
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
}
```

Now, let's say that we've added some hotkeys to our `HotkeyManager` (which we'll get to soon in another section) and we want to interact with them in another file. Well, we can do exactly that by calling the same method:

```csharp linenums="1" title="OtherViewModel.cs"
using HotkeyUtility;

public void InstantiateHotkeyManager()
{
    // This is the same instance as the one in the previous file
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
}
```

### Adding Hotkeys

If you want to programmatically add a hotkey to your application, you'll need to:

1. Create a `Hotkey` object
2. Use the `TryAddHotkey` method on `HotkeyManager` to add it

The `Hotkey` class has the following signature for its constructor:

```csharp linenums="1"
public Hotkey(Key key, ModifierKeys modifiers, EventHandler<HotkeyEventArgs> handler, ushort id = default)
```

You'll need to pass it a [Key](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key), a [ModifierKeys](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.modifierkeys), and an event handler that will be subscribed to the `Pressed` event that will trigger when the hotkey is pressed.

Once you've created your `Hotkey`, pass it to the `TryAddHotkey` method of an instance of the `HotkeyManager` class:

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    Key key = Key.A;
    ModifierKeys modifiers = ModifierKeys.Shift | ModifierKeys.Control;
    
    Hotkey hotkey = new(key, modifiers, Hotkey_Pressed);
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
    _ = hotkeyManager.TryAddHotkey(hotkey);
}

public static void Hotkey_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine();
}
```

The `TryAddHotkey` method returns a bool indicating whether or not the operation was successful.

### Removing Hotkeys

If you want to remove a hotkey from your application, you can do so by passing the same `Hotkey` object to the `TryRemoveHotkey` method of `HotkeyManager`:

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    Key key = Key.A;
    ModifierKeys modifiers = ModifierKeys.Shift | ModifierKeys.Control;
    
    Hotkey hotkey = new(key, modifiers, Hotkey_Pressed);
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
    _ = hotkeyManager.TryAddHotkey(hotkey);

    _ = hotkeyManager.TryRemoveHotkey(hotkey);
}

public static void Hotkey_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine();
}
```

Now, ++ctrl+shift+a++ will be unregistered from your application.

The `TryRemoveHotkey` method returns a bool indicating whether or not the operation was successful.

### Replacing Hotkeys

The `TryReplaceHotkey` method works by unregistering an existing `Hotkey` and reregistering it with a new [Key](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key), a new [ModifierKeys](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.modifierkeys), or both and returns a boolean indicating whether the operation was successful. This method consists of two overloaded methods. Here are both of their signatures:

```csharp linenums="1" title="First signature"
public bool TryReplaceHotkey(Key oldKey, ModifierKeys oldModifiers, Key newKey = Key.None, ModifierKeys newModifiers = ModifierKeys.None)
```

```csharp linenums="1" title="Second signature"
public bool TryReplaceHotkey(ushort id, Key newKey = Key.None, ModifierKeys newModifiers = ModifierKeys.None)
```

#### First Overloaded Method

For the first signature, all you need are the [Key](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key) and [ModifierKeys](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.modifierkeys) of an existing `Hotkey` object for the first two parameters. These will be used to iterate through the current hotkeys until it finds a matching Key and ModifierKeys. The `newKey` and `newModifiers` parameters will then be used to replace the binding of the existing `Hotkey`.

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
    Hotkey hotkey = new(Key.A, ModifierKeys.Shift | ModifierKeys.Control, Hotkey_Pressed);
    _ = hotkeyManager.TryAddHotkey(hotkey);

    // Now, pressing Shift + Control + A is no longer registered and Hotkey_Pressed
    // will no longer be triggered by that hotkey.
    // Instead, Hotkey_Pressed will now be triggered by pressing Alt + B.
    _ = hotkeyManager.TryReplaceHotkey(Key.A, ModifierKeys.Shift | ModifierKeys.Control, Key.B, ModifierKeys.Alt);
}

public static void Hotkey_Pressed(object sender, HotkeyEventArgs e)
{
    Console.WriteLine();
}
```

#### Second Overloaded Method

As for the second signature, the first parameter must be filled by the `Id` of an existing `Hotkey`. If the `Id` is a match, the `newKey` and `newModifiers` parameters will be used to replace the binding of the existing `Hotkey`.

```csharp linenums="1"
using System;
using System.Windows.Input;
using HotkeyUtility;

public static void Main()
{
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
    Hotkey hotkey = new(Key.A, ModifierKeys.Shift | ModifierKeys.Control, Hotkey_Pressed);
    _ = hotkeyManager.TryAddHotkey(hotkey);

    // Now, pressing Shift + Control + A is no longer registered and Hotkey_Pressed
    // will no longer be triggered by that hotkey.
    // Instead, Hotkey_Pressed will now be triggered by pressing Alt + B.
    hotkeyManager.TryReplaceHotkey(hotkey.Id, Key.B, ModifierKeys.Alt);
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
    HotkeyManager hotkeyManager = HotkeyManager.GetHotkeyManager();
    foreach (Hotkey hotkey in hotkeyManager.GetHotkeys())
    {
        // Do something here
    }
}
```

