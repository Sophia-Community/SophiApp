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
        /// <param name="script">Script to execute.</param>
        List<PSObject> Invoke(string script);

        /// <summary>
        /// Execute the script using version 5.1 of PowerShell and find out whether it was executed without errors.
        /// </summary>
        /// <param name="script">Script to execute.</param>
        /// <param name="result">The objects that the script returned.</param>
        /// <returns>False if the script wrote an error or was stopped by it.</returns>
        bool TryInvoke(string script, out List<PSObject> result);
    }
}
