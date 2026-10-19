// <copyright file="GreaterThanZeroToVisibility.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <inheritdoc/>
    public class GreaterThanZeroToVisibility : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is int intValue && intValue > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => DependencyProperty.UnsetValue;
    }
}
