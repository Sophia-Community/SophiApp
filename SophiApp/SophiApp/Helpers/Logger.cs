using SophiApp.Commons;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class Logger
    {
        private const string APPVERSION = "app_version";
        private const string COMPUTERNAME = "computer_name";
        private const string OSVERSION = "os_version";
        private const string USERDNSDOMAIN = "userdnsdomain";
        private const string USERDOMAIN = "user_domain";
        private const string USERNAME = "user_name";
        private List<string> logList = new List<string>();

        public Logger()
        {
            Initialize();
        }

        private void Initialize()
        {
            AddKeyValueString(APPVERSION, AppData.VersionString);
            AddKeyValueString(OSVERSION, $"{Environment.OSVersion}");
            AddKeyValueString(COMPUTERNAME, Environment.MachineName);
            AddKeyValueString(USERNAME, Environment.UserName);
            AddKeyValueString(USERDOMAIN, Environment.GetEnvironmentVariable(USERDNSDOMAIN));
        }

        internal void AddDateTimeValueString(LogType logString) => logList.Add($"[{DateTime.Now}] {logString}");

        internal void AddDateTimeValueString(LogType logString, string value) => logList.Add($"[{DateTime.Now}] {logString}:{value}");

        internal void AddKeyValueString(LogType key, string value) => logList.Add($"{key}:{value.ToUpper()}");

        internal void AddKeyValueString(string key, string value) => logList.Add($"{key.ToUpper()}:{value.ToUpper()}");
    }
}