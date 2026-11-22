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
        private static Task<Exception?>? check;

        /// <inheritdoc/>
        public List<PSObject> Invoke(string script) => Invoke(script, out _);

        /// <inheritdoc/>
        public void BeginCheck() => _ = StartCheck();

        /// <inheritdoc/>
        public Exception? GetCheckError() => StartCheck().GetAwaiter().GetResult();

        /// <inheritdoc/>
        public bool TryInvoke(string script, out List<PSObject> result)
        {
            try
            {
                result = Invoke(script, out var hadErrors);
                return !hadErrors;
            }
            catch (RuntimeException ex)
            {
                // Some errors stop the script and come as an exception, the others are only written to the error stream
                App.Logger.LogSkippedException($"The cmdlet \"{script}\"", ex);
                result = [];
                return false;
            }
        }

        // The check is run once, however many times it is asked for
        private static Task<Exception?> StartCheck()
        {
            lock (SessionLock)
            {
                return check ??= Task.Run(Check);
            }
        }

        // The cmdlets of the network modules are not asked for: those modules take the longest to load,
        // and they only wrap WMI classes, which are checked by themselves
        private static Exception? Check()
        {
            try
            {
                _ = Invoke("Get-Command -Name Get-AppxPackage, Get-WinLanguageBarOption, Enable-WindowsOptionalFeature -ErrorAction Stop", out var hadErrors);
                return hadErrors ? new InvalidOperationException("Windows PowerShell cannot find the cmdlets") : null;
            }
            catch (Exception ex)
            {
                return ex;
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
