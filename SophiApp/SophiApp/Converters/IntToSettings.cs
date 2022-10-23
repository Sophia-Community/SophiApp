using SophiApp.Commons;
using SophiApp.Customisations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.Converters
{
    internal class IntToSettings : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var localization = values[0] as Localization;
            var counter = System.Convert.ToString((values[1] as List<Customisation>).Count).ToCharArray();
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