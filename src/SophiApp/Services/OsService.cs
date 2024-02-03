// <copyright file="OsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class OsService : IOsService
    {
        /// <inheritdoc/>
        public void SetServiceStartMode(ServiceController service, ServiceStartMode mode)
        {
            var scManagerHandle = OpenSCManager(null, null, 0x000F003F);
            if (scManagerHandle == IntPtr.Zero)
            {
                throw new ExternalException($"Service {nameof(IOsService)} cannot open: Service Manager");
            }

            var serviceHandle = OpenService(scManagerHandle, service.ServiceName, 0x00000001 | 0x00000002);

            if (serviceHandle == IntPtr.Zero)
            {
                throw new ExternalException($"Service {nameof(IOsService)} cannot open service: \"{service.ServiceName}\"");
            }

            var result = ChangeServiceConfig(serviceHandle, 0xFFFFFFFF, (uint)mode, 0xFFFFFFFF, null, null, IntPtr.Zero, null, null, null, null);

            if (!result)
            {
                int nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);
                throw new ExternalException($"Service {nameof(IOsService)} could not change service \"{service.ServiceName}\" start type: {win32Exception.Message}");
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string? machineName, string? databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        private static extern bool ChangeServiceConfig(IntPtr hService, uint nServiceType, uint nStartType, uint nErrorControl, string? lpBinaryPathName, string? lpLoadOrderGroup, IntPtr lpdwTagId, [In] char[]? lpDependencies, string? lpServiceStartName, string? lpPassword, string? lpDisplayName);
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
    }
}
