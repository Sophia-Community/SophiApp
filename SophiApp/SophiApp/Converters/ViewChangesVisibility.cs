using SophiApp.Commons;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class ViewChangesVisibility : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var tag = values.First() as string;
            var counter = System.Convert.ToUInt32(values.Last());
            return tag != Tags.ViewSettings && counter > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}