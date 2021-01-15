using SophiAppCE.Commons;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SophiAppCE.Converters
{
    internal class MainWindowCloseButtonIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(System.Convert.ToByte(value) == Tags.StatusPageFinish);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}