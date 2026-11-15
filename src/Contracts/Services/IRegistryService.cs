// <copyright file="IRegistryService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using Microsoft.Win32;

    /// <summary>
    /// A service for working with <see cref="Registry"/> API.
    /// </summary>
    public interface IRegistryService
    {
        /// <summary>
        /// Delete saved locations for file types.
        /// </summary>
        void ClearCommonDialogViews();

        /// <summary>
        /// Removes the "StateFlags1337" key from the <see cref="Registry"/> path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VolumeCaches".
        /// </summary>
        void RemoveVolumeCachesStateFlags();

        /// <summary>
        /// Sets a DWORD registry value that is protected by the UCPD driver, using a copy of reg.exe under a different name.
        /// </summary>
        /// <param name="key">The full registry key path, e.g. HKCU\Software\Microsoft.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The data to be stored.</param>
        void SetRegistryValueBypassUCPD(string key, string name, int value);

        /// <summary>
        /// Sets the value of the "StateFlags1337" key in the <see cref="Registry"/> path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VolumeCaches".
        /// </summary>
        void SetVolumeCachesStateFlags();

        /// <summary>
        /// Instructs Windows to use the last installed CLR version.
        /// </summary>
        void UseLatestCLR();
    }
}
