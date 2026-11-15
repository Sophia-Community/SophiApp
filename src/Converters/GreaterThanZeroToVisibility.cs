// <copyright file="GreaterThanZeroToVisibility.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <summary>
    /// Shows an element when the number is greater than zero.
    /// </summary>
    public class GreaterThanZeroToVisibility : IValueConverter
    {
        /// <summary>
        /// Converts the source value for the element.
        /// </summary>
        /// <param name="value">The number.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns><see cref="Visibility.Visible"/> if the number is greater than zero, otherwise <see cref="Visibility.Collapsed"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is int intValue && intValue > 0 ? Visibility.Visible : Visibility.Collapsed;

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
