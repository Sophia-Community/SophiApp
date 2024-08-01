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
        /// Removes the "StateFlags1337" key from the <see cref="Registry"/> path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VolumeCaches".
        /// </summary>
        void RemoveVolumeCachesStateFlags();

        /// <summary>
        /// Sets the value of the "StateFlags1337" key in the <see cref="Registry"/> path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VolumeCaches".
        /// </summary>
        void SetVolumeCachesStateFlags();
    }
}
