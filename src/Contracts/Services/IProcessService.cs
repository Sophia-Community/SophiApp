// <copyright file="IProcessService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Diagnostics;

    /// <summary>
    /// A service for working with Windows <see cref="Process"/> API.
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Close eventvwr.msc console if is running.
        /// </summary>
        void CloseEventViewerConsole();

        /// <summary>
        /// Determines whether the specified process exists.
        /// </summary>
        /// <param name="name">Process name.</param>
        bool Exist(string name);

        /// <summary>
        /// Determines whether the specified process exists.
        /// </summary>
        /// <param name="process">Processes name.</param>
        bool Exist(params string[] process);

        /// <summary>
        /// Immediately stops the process.
        /// </summary>
        /// <param name="name">Process name.</param>
        /// <param name="timeout">Time, in milliseconds, to wait for the process to complete.</param>
        void KillProcessByName(string name, int timeout = 1000);

        /// <summary>
        /// Immediately stops the processes.
        /// </summary>
        /// <param name="timeout">Time, in milliseconds, to wait for the process to complete.</param>
        /// <param name="processes">Processes name.</param>
        void KillProcessByName(int timeout, params string[] processes);

        /// <summary>
        /// Set File Explorer process automatically restart property.
        /// </summary>
        /// <param name="allow">Allow or deny automatically restart.</param>
        void SetAutoRestartShell(bool allow);

        /// <summary>
        /// Start associated process indefinitely.
        /// </summary>
        /// <param name="name">A application or document to start.</param>
        /// <param name="arguments">A arguments to use when starting the application or document.</param>
        /// <param name="style">Specified how a new window should appear when the system starts a process.</param>
        Process? StartProcessByName(string name, string arguments = "", ProcessWindowStyle style = ProcessWindowStyle.Normal);

        /// <summary>
        /// Start and wait the associated process to exit.
        /// </summary>
        /// <param name="name">A application or document to start.</param>
        /// <param name="arguments">A arguments to use when starting the application or document.</param>
        Process WaitForExit(string name, string arguments);

        /// <summary>
        /// Start and wait the associated process to exit.
        /// </summary>
        /// <param name="name">A application or document to start.</param>
        void WaitForExit(string name);
    }
}
