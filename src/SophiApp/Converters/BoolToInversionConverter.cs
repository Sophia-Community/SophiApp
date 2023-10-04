// <copyright file="BoolToInversionConverter.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    /// <inheritdoc/>
    public class BoolToInversionConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool ? !(bool)value : throw new ArgumentException("ExceptionBoolToInversionConverterValueMustBeBool");

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => DependencyProperty.UnsetValue;
    }
}
