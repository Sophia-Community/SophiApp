using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class Logger
    {
        private List<string> logList = new List<string>();

        private const string APPVERSION = "app_version";
        private const string OSVERSION = "os_version";
        private const string COMPUTERNAME = "computer_name";
        private const string USERNAME = "user_name";
        private const string USERDOMAIN = "user_domain";
        private const string USERDNSDOMAIN = "userdnsdomain";

        public Logger()
        {
            Initialize();
        }

        //private string DateString
        //{
        //    set => logList.Add($"{DateTime.Now} {value.ToUpper()}");
        //}

        //private string ValueString
        //{
        //    set => logList.Add(value.ToUpper());
        //}

        private void Initialize()
        {
            ValueString(APPVERSION, AppData.VersionString);
            ValueString(OSVERSION, $"{Environment.OSVersion}");
            ValueString(COMPUTERNAME, Environment.MachineName);            
            ValueString(USERNAME, Environment.UserName);
            ValueString(USERDOMAIN, Environment.GetEnvironmentVariable(USERDNSDOMAIN));
        }

        internal void ValueString(LogType key, string value) => logList.Add($"{key}={value.ToUpper()}");
        internal void ValueString(string key, string value) => logList.Add($"{key.ToUpper()}={value.ToUpper()}");


        internal void DateString(LogType logString) => logList.Add($"{DateTime.Now} {logString}");

        internal void DateString(LogType logString, string value) => logList.Add($"{logString}:{value}");
    }
}
