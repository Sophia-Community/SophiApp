// <copyright file="ProcessService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <inheritdoc/>
    public class ProcessService : IProcessService
    {
        /// <inheritdoc/>
        public Process InvokeAsCmd(string command)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {command}";
            _ = process.Start();
            process.WaitForExit();
            return process;
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
        public bool ProcessExist(string name)
        {
            return Array.Exists(Process.GetProcessesByName(name), process => process.ProcessName.Equals(name));
        }
    }
}
