// <copyright file="PageTagToHitTestVisible.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using SophiApp.Helpers;
    using SophiApp.UI;

    /// <summary>
    /// Defines the IsHitTestVisible property of <see cref="CategoryMenu"/>.
    /// </summary>
    public class PageTagToHitTestVisible : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (PageTag)value != PageTag.Busy;

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
    }
}
