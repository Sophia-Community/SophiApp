using System;
using System.Collections.Generic;

namespace SophiApp.Commons
{
    internal class Logger
    {
        private List<string> logList = new List<string>();

        public Logger()
        {
            Collect();
        }

        private string DateString
        {
            set => logList.Add($"{DateTime.Now} {value.ToUpper()}");
        }

        private string ValueString
        {
            set => logList.Add(value.ToUpper());
        }

        internal void Collect()
        {
            ValueString = $"osversion={Environment.OSVersion}";
            ValueString = $"computername={Environment.MachineName}";
            ValueString = $"userdomain={Environment.GetEnvironmentVariable("userdnsdomain")}";
            ValueString = $"username={Environment.UserName}";
        }

        internal void Collect(LogType logString) => DateString = $"{logString}";

        internal void Collect(LogType logString, string value) => DateString = $"{logString}:{value}";
    }
}