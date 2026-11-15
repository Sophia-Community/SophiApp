// <copyright file="PowerShellService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    /// <inheritdoc/>
    public class PowerShellService : IPowerShellService
    {
        private static readonly Lock SessionLock = new();
        private static PowerShellProcessInstance? sharedProcess;
        private static Runspace? sharedRunspace;
        private static bool processExitSubscribed;

        /// <inheritdoc/>
        public List<PSObject> Invoke(string script) => Invoke(script, out _);

        /// <inheritdoc/>
        public bool TryInvoke(string script, out List<PSObject> result)
        {
            try
            {
                result = Invoke(script, out var hadErrors);
                return !hadErrors;
            }
            catch (RuntimeException)
            {
                // Some errors stop the script and come as an exception, the others are only written to the error stream
                result = [];
                return false;
            }
        }

        private static List<PSObject> Invoke(string script, out bool hadErrors)
        {
            // All scripts are run one by one in the same Windows PowerShell process: starting a new process for every call takes up to a second
            lock (SessionLock)
            {
                using var powerShell = PowerShell.Create();
                powerShell.Runspace = GetSharedRunspace();

                // The script is run in its own scope, so its variables do not affect the next scripts
                var invokeResult = powerShell.AddScript(script, useLocalScope: true).Invoke();
                hadErrors = powerShell.HadErrors;
                return [..invokeResult];
            }
        }

        private static Runspace GetSharedRunspace()
        {
            if (sharedRunspace?.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                return sharedRunspace;
            }

            // The session is created on the first call, or created again if the PowerShell process was closed
            CloseSharedRunspace();
            sharedProcess = new PowerShellProcessInstance(new Version(5, 1), null, ScriptBlock.Create("Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force"), false);
            sharedRunspace = RunspaceFactory.CreateOutOfProcessRunspace(new TypeTable(Array.Empty<string>()), sharedProcess);
            sharedRunspace.Open();

            if (!processExitSubscribed)
            {
                // Close the PowerShell process together with the app
                AppDomain.CurrentDomain.ProcessExit += (_, _) => CloseSharedRunspace();
                processExitSubscribed = true;
            }

            return sharedRunspace;
        }

        private static void CloseSharedRunspace()
        {
            try
            {
                sharedRunspace?.Dispose();
                sharedProcess?.Dispose();
            }
            catch (Exception)
            {
                // The PowerShell process has already exited
            }
            finally
            {
                sharedRunspace = null;
                sharedProcess = null;
            }
        }
    }
}
