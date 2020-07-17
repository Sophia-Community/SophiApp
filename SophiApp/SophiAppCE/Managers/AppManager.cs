using SophiAppCE.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SophiAppCE.Managers
{
    internal class AppManager
    {
        internal string AppBaseDir { get; } = AppDomain.CurrentDomain.BaseDirectory;
        private List<JsonData> SettingsJsonData { get; set; }
        public AppManager()
        {
            FillSettingsJsonData();
        }

        private void FillSettingsJsonData()
        {
            //char[] splitters = new char[] { '\r', ':' };
            //string jsonData = Encoding.UTF8.GetString(Properties.Resources.SettingsCE);
            //List<string> jsonStrings = Regex.Matches(jsonData, @"\{(.*?)\}", RegexOptions.Compiled | RegexOptions.Singleline)
            //                                .Cast<Match>()
            //                                .Select(s => Convert.ToString(s).Replace("\r\n", ""))
            //                                .ToList();
        }
    }
}
