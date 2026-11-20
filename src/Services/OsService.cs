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
        public ServiceControllerStatus GetStatus(string name)
        {
            using var service = new ServiceController(name);
            return service.Status;
        }

        /// <inheritdoc/>
        public void SetStartMode(ServiceController service, ServiceStartMode mode)
        {
            var scManagerHandle = OpenSCManager(null, null, 0x000F003F);
            if (scManagerHandle == nint.Zero)
            {
                throw new ExternalException($"Service {nameof(IOsService)} cannot open: Service Manager");
            }

            var serviceHandle = OpenService(scManagerHandle, service.ServiceName, 0x00000001 | 0x00000002);

            if (serviceHandle == nint.Zero)
            {
                throw new ExternalException($"Service {nameof(IOsService)} cannot open service: {service.ServiceName}");
            }

            var result = ChangeServiceConfig(serviceHandle, 0xFFFFFFFF, (uint)mode, 0xFFFFFFFF, null, null, nint.Zero, null, null, null, null);

            if (!result)
            {
                int nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);
                throw new ExternalException($"Service {nameof(IOsService)} could not change service {service.ServiceName} start type: {win32Exception.Message}");
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }

        /// <inheritdoc/>
        public bool Exist(string name)
        {
            try
            {
                using var service = new ServiceController(name);
                _ = service.Status;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public void TrySetStartMode(string name, ServiceStartMode mode)
        {
            try
            {
                using var service = new ServiceController(name);
                SetStartMode(service, mode);
            }
            catch (Exception ex)
            {
                // The service is missing or cannot be changed: the app goes on
                App.Logger.LogSkippedException($"Setting the {mode} start mode of the {name} service", ex);
            }
        }

        /// <inheritdoc/>
        public void TryStart(string name)
        {
            try
            {
                using var service = new ServiceController(name);

                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                }
            }
            catch (Exception ex)
            {
                // The service is missing or cannot be started: the app goes on
                App.Logger.LogSkippedException($"Starting the {name} service", ex);
            }
        }

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern int CloseServiceHandle(nint hSCObject);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern nint OpenSCManager(string? machineName, string? databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern nint OpenService(nint hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ChangeServiceConfig(nint hService, uint nServiceType, uint nStartType, uint nErrorControl, string? lpBinaryPathName, string? lpLoadOrderGroup, nint lpdwTagId, [In] char[]? lpDependencies, string? lpServiceStartName, string? lpPassword, string? lpDisplayName);

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        private static extern int HashData(byte[] pbData, int cbData, byte[] piet, int outputLen);
    }
}
