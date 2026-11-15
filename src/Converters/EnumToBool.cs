// <copyright file="EnumToBool.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Tells whether an enumeration value is the one named in the converter parameter.
    /// </summary>
    public class EnumToBool : IValueConverter
    {
        /// <summary>
        /// Converts the source value for the element.
        /// </summary>
        /// <param name="value">The enumeration value.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>True if the value has the name passed in the parameter.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
            => value?.ToString()?.Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;

        /// <summary>
        /// The reverse conversion is not used.
        /// </summary>
        /// <param name="value">The value of the element.</param>
        /// <param name="targetType">The type of the source property.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns><see cref="DependencyProperty.UnsetValue"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => DependencyProperty.UnsetValue;
    }
}
