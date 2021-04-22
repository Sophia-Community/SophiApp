using System;

namespace SophiApp.Commons
{
    internal class Localization
    {
        public Localization(string text, Uri uri, UILanguage language)
        {
            Text = text;
            Uri = uri;
            Language = language;
        }

        public Localization()
        {
        }

        internal UILanguage Language { get; set; }
        internal string Text { get; set; }
        internal Uri Uri { get; set; }
    }
}