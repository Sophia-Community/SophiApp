// <copyright file="IPowerShellService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Management.Automation;

    /// <summary>
    /// A service for working with Windows PowerShell API.
    /// </summary>
    public interface IPowerShellService
    {
        /// <summary>
        /// Execute the script using version 5.1 of PowerShell.
        /// </summary>
        /// <typeparam name="T">Return object type.</typeparam>
        /// <param name="script">Script to execute.</param>
        T Invoke<T>(string script)
            where T : struct;

        /// <summary>
        /// Execute the script using version 5.1 of PowerShell.
        /// </summary>
        /// <param name="script">Script to execute.</param>
        List<PSObject> Invoke(string script);
    }
}
