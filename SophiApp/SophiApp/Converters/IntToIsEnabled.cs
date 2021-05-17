using System;
using System.Globalization;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class IntToIsEnabled : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => System.Convert.ToUInt32(value) > 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}