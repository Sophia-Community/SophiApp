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
        /// Determines whether the specified process exists.
        /// </summary>
        /// <param name="name">Process name.</param>
        bool ProcessExist(string name);

        /// <summary>
        /// Immediately stops the all associated process.
        /// </summary>
        /// <param name="name">Process name.</param>
        /// <param name="timeout">Time, in milliseconds, to wait for the process to complete.</param>
        void KillAllProcesses(string name, int timeout = 1000);

        /// <summary>
        /// Start associated process indefinitely.
        /// </summary>
        /// <param name="name">A application or document to start.</param>
        /// <param name="arguments">A arguments to use when starting the application or document.</param>
        /// <param name="style">Specified how a new window should appear when the system starts a process.</param>
        Process? Start(string name, string arguments = "", ProcessWindowStyle style = ProcessWindowStyle.Normal);

        /// <summary>
        /// Start and wait the associated process to exit.
        /// </summary>
        /// <param name="name">A application or document to start.</param>
        /// <param name="arguments">A arguments to use when starting the application or document.</param>
        Process WaitForExit(string name, string arguments);
    }
}
