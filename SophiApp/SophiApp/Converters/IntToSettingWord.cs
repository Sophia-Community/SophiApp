using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class IntToSettingWord : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var localization = (Localization)values[0];
            var counter = System.Convert.ToString(values[1]).ToCharArray();
            var word = values[2] as string;

            if (localization.Language == UILanguage.RU)
            {
                if (counter.Length == 2 && counter.First() == '1')
                {
                    return "настроек";
                }

                switch (counter.Last())
                {
                    case '1':
                        return "настройка";

                    case '2':
                    case '3':
                    case '4':
                        return "настройки";

                    default:
                        return "настроек";
                }
            }

            return word;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
