using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Commons
{
    internal class Localization
    {
        public Localization(string text, Uri uri, UILanguage language)
        {
            Name = text;
            Uri = uri;
            Language = language;
        }

        public Localization()
        {
        }

        internal UILanguage Language { get; set; }
        internal Uri Uri { get; set; }
        public string Name { get; set; }
    }
}
