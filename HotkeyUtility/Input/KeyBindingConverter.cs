using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace HotkeyUtility.Input
{
    /// <summary>
    /// Converts a <see cref="KeyBinding"/> object to and from other types.
    /// </summary>
    public class KeyBindingConverter : TypeConverter
    {
        private const char DisplaystringSeparator = ',';

        private const char ModifiersDelimiter = '+';

        private static readonly KeyConverter keyConverter = new();

        private static readonly ModifierKeysConverter modifierKeysConverter = new();

        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // We can only handle string.
            return sourceType == typeof(string);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            // We can convert to an InstanceDescriptor or to a string.
            if (destinationType == typeof(string))
            {
                // When invoked by the serialization engine we can convert to string only for known type
                if (context?.Instance is KeyBinding keyBinding)
                {
                    return ModifierKeysConverter.IsDefinedModifierKeys(keyBinding.Modifiers) && IsDefinedKey(keyBinding.Key);
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (source is string s)
            {
                KeyBinding keyBinding = new();

                string fullName = s.Trim();
                if (!(fullName.Length > 0))
                {
                    keyBinding.Key = Key.None;
                    return keyBinding;
                }

                string keyToken;
                string modifiersToken;
                string displayString;

                // break apart display string
                int index = fullName.IndexOf(DisplaystringSeparator);
                if (index >= 0)
                {
                    displayString = fullName[(index + 1)..].Trim();
                    fullName = fullName[..index].Trim();
                }
                else
                {
                    displayString = string.Empty;
                }

                // break apart key and modifiers
                index = fullName.LastIndexOf(ModifiersDelimiter);
                if (index >= 0)
                {
                    // modifiers exists
                    modifiersToken = fullName[..index];
                    keyToken = fullName[(index + 1)..];
                }
                else
                {
                    modifiersToken = string.Empty;
                    keyToken = fullName;
                }

                ModifierKeys modifiers = ModifierKeys.None;
                object resultkey = keyConverter.ConvertFrom(context, culture, keyToken);
                if (resultkey != null)
                {
                    object temp = modifierKeysConverter.ConvertFrom(context, culture, modifiersToken);
                    if (temp != null)
                    {
                        modifiers = (ModifierKeys)temp;
                    }

                    keyBinding.Key = (Key)resultkey;
                    keyBinding.Modifiers = modifiers;
                    return keyBinding;
                }
            }

            throw GetConvertFromException(source);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    if (value is KeyBinding keyBinding)
                    {
                        if (keyBinding.Key == Key.None)
                        {
                            return string.Empty;
                        }

                        string strBinding = string.Empty;
                        string strKey = (string)keyConverter.ConvertTo(context, culture, keyBinding.Key, destinationType);
                        if (strKey.Length > 0)
                        {
                            strBinding += modifierKeysConverter.ConvertTo(context, culture, keyBinding.Modifiers, destinationType) as string;
                            if (strBinding.Length > 0)
                            {
                                strBinding += ModifiersDelimiter;
                            }

                            strBinding += strKey;
                        }

                        return strBinding;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }

            throw GetConvertToException(value, destinationType);
        }

        /// <summary>
        /// Checks for a valid enum, as any <see cref="int"/> can be casted to the enum.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns><see langword="true"/> if the specified key falls between the values of <see cref="Key.None"/> and <see cref="Key.OemClear"/>; otherwise, <see langword="false"/>.</returns>
        internal static bool IsDefinedKey(Key key)
        {
            return key is >= Key.None and <= Key.OemClear;
        }
    }
}
