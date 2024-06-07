// <copyright file="PowerShellService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class PowerShellService : IPowerShellService
    {
        /// <inheritdoc/>
        public T Invoke<T>(string script)
            where T : struct
        {
            return (T)Invoke(script.Insert(0, "Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force;"))[0].BaseObject;
        }

        /// <inheritdoc/>
        public List<PSObject> Invoke(string script)
        {
            using var runspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(Array.Empty<string>()), new PowerShellProcessInstance(new Version(5, 1), null, null, false));
            runspace.Open();
            using var instance = PowerShell.Create(runspace).AddScript(script.Insert(0, "Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force;"));
            return [.. instance.Invoke()];
        }
    }
}
