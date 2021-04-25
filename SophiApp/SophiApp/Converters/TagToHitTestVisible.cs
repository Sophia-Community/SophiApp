using SophiApp.Commons;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class TagToHitTestVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value as string != Tags.InfoPanelLoading;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}