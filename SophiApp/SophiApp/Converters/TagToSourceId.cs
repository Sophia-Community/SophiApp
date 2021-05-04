using SophiApp.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class TagToSourceId : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (value as ListCollectionView).Cast<BaseTextedElement>().ToList();
            var param = (parameter as string).Split('_')[1];
            var id = System.Convert.ToUInt32(param);
            return list.Where(element => element.Id == id);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}