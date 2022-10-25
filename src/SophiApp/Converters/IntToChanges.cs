using SophiApp.Commons;
using SophiApp.Customisations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SophiApp.Converters
{
    internal class IntToChanges : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var localization = values[0] as Localization;
            var counter = System.Convert.ToString((values[1] as List<Customisation>).Count).ToCharArray();
            var word = values[2] as string;

            if (localization.Language == UILanguage.RU)
            {
                var eleven = new char[] { '1', '1' };

                if (counter.SequenceEqual(eleven))
                    return "Изменено";

                switch (counter.Last())
                {
                    case '1':
                        return "Изменена";

                    default:
                        return "Изменено";
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