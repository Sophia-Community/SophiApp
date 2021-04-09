using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class ObjectToIntList : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values.Cast<int>().ToList();

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}