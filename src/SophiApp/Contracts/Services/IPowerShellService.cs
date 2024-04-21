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
        /// Execute the PowerShell script.
        /// </summary>
        /// <typeparam name="T">Return object type.</typeparam>
        /// <param name="script">Script to execute.</param>
        T Invoke<T>(string script)
            where T : struct;

        /// <summary>
        /// Execute the PowerShell script.
        /// </summary>
        /// <param name="script">Script to execute.</param>
        ICollection<PSObject> Invoke(string script);
    }
}
