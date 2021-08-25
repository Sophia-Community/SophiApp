using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class StringToIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Application.Current.TryFindResource(value);


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
