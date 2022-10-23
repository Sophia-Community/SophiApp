using System;

namespace SophiApp.Commons
{
    internal class Theme
    {
        public Theme(Uri uri, string name, string alias)
        {
            Uri = uri;
            Name = name;
            Alias = alias;
        }

        public Theme()
        {
        }

        public string Alias { get; set; }
        public string Name { get; set; }
        public Uri Uri { get; set; }
    }
}