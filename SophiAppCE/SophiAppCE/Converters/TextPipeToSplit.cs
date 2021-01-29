using SophiAppCE.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SophiAppCE.Converters
{
    class TextPipeToSplit : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string str = values.First() as string ?? string.Empty;
            string[] array = str.Split(Constants.PipeDelimiter);
            byte index = System.Convert.ToByte(values.Last());
            return str.Contains(Constants.PipeDelimiter) == true ? array[index] : str ;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
