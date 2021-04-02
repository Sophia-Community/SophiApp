using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class TagToVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var activeTag = values.First() as string;
            var elementTag = values.Last() as string;
            return activeTag == elementTag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}