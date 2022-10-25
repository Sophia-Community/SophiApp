using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}