using SophiApp.Commons;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class LogManager
    {
        private const string APPVERSION = "app_version";
        private const string COMPUTERNAME = "computer_name";
        private const string OSVERSION = "os_version";
        private const string USERDNSDOMAIN = "userdnsdomain";
        private const string USERDOMAIN = "user_domain";
        private const string USERNAME = "user_name";
        private List<string> logList = new List<string>();

        public LogManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            var osVer = OsManager.GetProductName() ?? $"{Environment.OSVersion}";
            var userDomain = Environment.GetEnvironmentVariable(USERDNSDOMAIN) ?? Environment.UserDomainName;

            AddKeyValueString(APPVERSION, $"{AppDataManager.Version}");
            AddKeyValueString(OSVERSION, osVer);
            AddKeyValueString(COMPUTERNAME, Environment.MachineName);
            AddKeyValueString(USERNAME, Environment.UserName);
            AddKeyValueString(USERDOMAIN, userDomain);
            AddKeyValueString(LogType.APP_STARTUP_DIR, $"{AppDataManager.StartupFolder}");
        }

        internal void AddDateTimeValueString(LogType logString) => logList.Add($"[{DateTime.Now}] {logString}");

        internal void AddDateTimeValueString(LogType logString, string value) => logList.Add($"[{DateTime.Now}] {logString}:{value.ToUpper()}");

        internal void AddDateTimeValueString(LogType logString, string id, string state) => logList.Add($"[{DateTime.Now}] {logString}:{id.ToUpper()}:{state.ToUpper()}");

        internal void AddKeyValueString(LogType key, string value) => logList.Add($"{key}:{value.ToUpper()}");

        internal void AddKeyValueString(string key, string value) => logList.Add($"{key.ToUpper()}:{value.ToUpper()}");

        internal void AddSeparator() => logList.Add(string.Empty);

        internal List<string> GetLog() => logList;
    }
}