// <copyright file="ProcessService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using System.Management.Automation.Runspaces;
    using System.Xml.Linq;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using WinUIEx;

    /// <inheritdoc/>
    public class ProcessService : IProcessService
    {
        /// <inheritdoc/>
        public bool ProcessExist(string name)
        {
            return Array.Exists(Process.GetProcessesByName(name), process => process.ProcessName.Equals(name));
        }

        /// <inheritdoc/>
        public void KillAllProcesses(string name, int timeout = 1000)
        {
            Process.GetProcessesByName(name)
                .ForEach(process =>
                {
                    process.Kill();
                    process.WaitForExit(timeout);
                    process.Dispose();
                });
        }

        /// <inheritdoc/>
        public Process WaitForExit(string name, string arguments)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = name;
            process.StartInfo.Arguments = arguments;
            _ = process.Start();
            process.WaitForExit();
            return process;
        }

        /// <inheritdoc/>
        public Process? Start(string name, string arguments = "", ProcessWindowStyle style = ProcessWindowStyle.Normal)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = name,
                Arguments = arguments,
                WindowStyle = style,
            });
        }
    }
}
