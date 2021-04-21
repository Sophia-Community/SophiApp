using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Commons
{
    internal class Localization
    {
        internal string Text { get; set; }
        internal Uri Uri { get; set; }
        internal UILanguage Language { get; set; }

        public Localization(string text, Uri uri, UILanguage language)
        {
            Text = text;
            Uri = uri;
            Language = language;
        }

        public Localization() { }
    }
}
