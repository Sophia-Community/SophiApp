using System;

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

        public string Name { get; set; }
        internal UILanguage Language { get; set; }
        internal Uri Uri { get; set; }
    }
}