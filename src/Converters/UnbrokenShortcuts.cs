// <copyright file="UnbrokenShortcuts.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;
    using SophiApp.Helpers;

    /// <summary>
    /// Shows a text so that key combinations such as "Win+Shift+S" are not broken between lines.
    /// </summary>
    public class UnbrokenShortcuts : IValueConverter
    {
        /// <summary>
        /// Converts the source value for the element.
        /// </summary>
        /// <param name="value">The text.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The text to show.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is string text ? FormattedTextBlock.KeepShortcutsOnOneLine(text) : value;

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
