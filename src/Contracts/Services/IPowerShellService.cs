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
        /// Delete saved locations for file types.
        /// </summary>
        void ClearCommonDialogViews();

        /// <summary>
        /// Checks if there are any connected network adapters with the managed property "AllowComputerToTurnOffDevice".
        /// </summary>
        bool? TurnOffDeviceNetworkAdapterExist();

        /// <summary>
        /// Return <paramref name="script"/> execute result or default.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="script">Script to execute.</param>
        T? InvokeOrDefault<T>(string script)
            where T : struct;

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

        /// <summary>
        /// Execute the command bypassing the UCPD driver.
        /// </summary>
        /// <param name="command">The command to be execute, must begin with "-Command".</param>
        void InvokeCommandBypassUCPD(string command);

        /// <summary>
        /// Set network adapters managed property "AllowComputerToTurnOffDevice" state.
        /// </summary>
        /// <param name="enable">Network adapters "AllowComputerToTurnOffDevice" property state.</param>
        void SetTurnOffDeviceNetworkAdapterState(bool enable);
    }
}
