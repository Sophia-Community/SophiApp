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
        /// Immediately stops the all associated process.
        /// </summary>
        /// <param name="name">Process name.</param>
        /// <param name="timeout">Time, in milliseconds, to wait for the process to complete.</param>
        void KillAllProcesses(string name, int timeout = 1000);

        /// <summary>
        /// Determines whether the specified process exists.
        /// </summary>
        /// <param name="name">Process name.</param>
        bool ProcessExist(string name);

        /// <summary>
        /// Invoke the string as a cmd command.
        /// </summary>
        /// <param name="command">String command to be executed.</param>
        Process InvokeAsCmd(string command);
    }
}
