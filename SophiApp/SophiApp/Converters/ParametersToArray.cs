using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class ParametersToArray : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => new List<uint> { System.Convert.ToUInt32(values[0]), System.Convert.ToUInt32(values[1]) };

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}