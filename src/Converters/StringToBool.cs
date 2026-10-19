// <copyright file="StringToBool.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using Microsoft.UI.Xaml.Data;
    using System;

    /// <inheritdoc/>
    public class StringToBool : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
            => (string)value == (string)parameter;

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => (string)parameter;
    }
}
