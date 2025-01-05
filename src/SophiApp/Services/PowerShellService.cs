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
        private readonly IProcessService processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellService"/> class.
        /// </summary>
        /// <param name="processService">A service for working with Windows <see cref="Process"/> API.</param>
        public PowerShellService(IProcessService processService)
        {
            this.processService = processService;
        }

        /// <inheritdoc/>
        public void ClearCommonDialogViews()
        {
            var command = "Get-ChildItem -Path \"HKCU:\\Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\Shell\\Bags\\*\\Shell\" -Recurse | Where-Object -FilterScript {$_.PSChildName -eq \"{885A186E-A440-4ADA-812B-DB871B942259}\"} | Remove-Item -Force";
            _ = Invoke(command);
        }

        /// <inheritdoc/>
        public bool GetMsDefenderPreferenceException()
        {
            var command = @"try
{
    Get-MpPreference -ErrorAction Stop | Out-Null
    return $false
}
catch
{
    return $true
}";
            return Invoke<bool>(command);
        }

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

        /// <inheritdoc/>
        public void InvokeCommandBypassUCPD(string command)
        {
            var systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var powershell = Path.Combine(systemRoot, "WindowsPowerShell\\v1.0\\powershell.exe");
            var powershellTemp = Path.Combine(systemRoot, "WindowsPowerShell\\v1.0\\powershell_temp.exe");
            File.Copy(powershell, powershellTemp, true);
            _ = processService.WaitForExit(name: powershellTemp, arguments: command);
            File.Delete(powershellTemp);
        }
    }
}
