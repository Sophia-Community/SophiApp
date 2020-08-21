using SophiAppCE.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace SophiAppCE.Converters
{
    class SwitchSelectAllConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<SwitchBarModel> switchBarModels = values[0] as ObservableCollection<SwitchBarModel>;
            string stateSwitchTag = values[1] as string;
            return switchBarModels.Where(s => s.Tag == stateSwitchTag).Count() > 3 ? Visibility.Visible : Visibility.Hidden ;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue as object[];
        }
    }
}
