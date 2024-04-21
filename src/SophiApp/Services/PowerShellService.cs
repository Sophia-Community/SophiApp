// <copyright file="PowerShellService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Management.Automation;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class PowerShellService : IPowerShellService
    {
        /// <inheritdoc/>
        public T Invoke<T>(string script)
            where T : struct
        {
            return (T)PowerShell.Create().AddScript(script).Invoke()[0].BaseObject;
        }

        /// <inheritdoc/>
        public ICollection<PSObject> Invoke(string script)
        {
            return PowerShell.Create().AddScript(script).Invoke();
        }
    }
}
