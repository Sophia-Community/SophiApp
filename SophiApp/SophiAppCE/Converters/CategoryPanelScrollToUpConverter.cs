using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SophiAppCE.Converters
{
    class CategoryPanelScrollToUpConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string clickedButtonTag = System.Convert.ToString(values.FirstOrDefault());
            string viewPanelTag = System.Convert.ToString(values.LastOrDefault());
            return clickedButtonTag == viewPanelTag ? true : false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
