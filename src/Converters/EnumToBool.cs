// <copyright file="EnumToBool.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <inheritdoc/>
    public class EnumToBool : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
            => value?.ToString()?.Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => DependencyProperty.UnsetValue;
    }
}
