using SophiApp.Commons;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class Debugger2
    {
        private List<string> log = new List<string>();

        public Debugger2()
        {
            Init();
        }

        private void Init()
        {
            InitialWrite(DebuggerRecord.APP_VERSION, $"{AppData.Version}");
            InitialWrite(DebuggerRecord.STARTUP_DIR, $"{AppData.StartupFolder}");
            InitialWrite(DebuggerRecord.OS_NAME, $"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()}");
            InitialWrite(DebuggerRecord.OS_BUILD, Environment.OSVersion.VersionString);
            InitialWrite(DebuggerRecord.OS_ORG, $"{OsHelper.GetRegisteredOrganization()}");
            InitialWrite(DebuggerRecord.OS_OWNER, $"{OsHelper.GetRegisteredOwner()}");
            InitialWrite(DebuggerRecord.COMPUTER_NAME, Environment.MachineName);
            InitialWrite(DebuggerRecord.USER_NAME, Environment.UserName);
            InitialWrite(DebuggerRecord.USER_DOMAIN, Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName);
        }

        private void InitialWrite(DebuggerRecord key, string value) => log.Add($"{key}:{value.ToUpper()}");

        internal List<string> GetLog() => log;

        internal void Write(DebuggerRecord record) => log.Add($"[{DateTime.Now}] {record}");

        internal void Write(DebuggerRecord record, string value) => log.Add($"[{DateTime.Now}] {record}:{value.ToUpper()}");

        internal void Write(DebuggerRecord record, string id, string state) => log.Add($"[{DateTime.Now}] {record}:{id.ToUpper()}:{state.ToUpper()}");
    }
}