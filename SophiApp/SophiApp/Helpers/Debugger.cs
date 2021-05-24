using SophiApp.Commons;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class Debugger
    {
        private List<string> log = new List<string>();

        public Debugger()
        {
            Init();
        }

        private void Init()
        {
            InitRecord(DebuggerRecord.VERSION, $"{AppData.Version}");
            InitRecord(DebuggerRecord.STARTUP_DIR, $"{AppData.StartupFolder}");
            InitRecord(DebuggerRecord.OS_VERSION, OsManager.GetProductName() ?? $"{Environment.OSVersion}");
            InitRecord(DebuggerRecord.COMPUTER_NAME, Environment.MachineName);
            InitRecord(DebuggerRecord.USER_NAME, Environment.UserName);
            InitRecord(DebuggerRecord.USER_DOMAIN, Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName);
            AddRecord();
        }

        internal void AddRecord() => log.Add(string.Empty);

        internal void AddRecord(DebuggerRecord record) => log.Add($"[{DateTime.Now}] {record}");

        internal void AddRecord(DebuggerRecord record, string value) => log.Add($"[{DateTime.Now}] {record}:{value.ToUpper()}");

        internal List<string> GetLog() => log;

        internal void InitRecord(DebuggerRecord key, string value) => log.Add($"{key}:{value.ToUpper()}");
    }
}