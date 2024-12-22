// <copyright file="OsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using Microsoft.Win32;
    using System.Text;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class OsService : IOsService
    {
        /// <inheritdoc/>
        public uint GetNewsInterestsHashData(bool enable)
        {
            var clientPath = "Software\\Microsoft\\SQMClient";
            var machineId = Registry.LocalMachine.OpenSubKey(clientPath)?.GetValue("MachineId") as string ?? string.Empty;
            var combinedId = $"{machineId}_{(enable ? 0 : 2)}".ToCharArray();
            Array.Reverse(combinedId);
            var bytesIn = Encoding.Unicode.GetBytes(new string(combinedId));
            var bytesOut = new byte[4];
            _ = HashData(bytesIn, 0x53, bytesOut, bytesOut.Length);
            return BitConverter.ToUInt32(bytesOut, 0);
        }

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

        /// <inheritdoc/>
        public bool IsServiceExist(string service)
        {
            try
            {
                using var serviceController = new ServiceController(service);
                return serviceController.ServiceName.Equals(service);
            }
            catch (Exception)
            {
                return false;
            }
        }

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string? machineName, string? databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ChangeServiceConfig(IntPtr hService, uint nServiceType, uint nStartType, uint nErrorControl, string? lpBinaryPathName, string? lpLoadOrderGroup, IntPtr lpdwTagId, [In] char[]? lpDependencies, string? lpServiceStartName, string? lpPassword, string? lpDisplayName);

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        private static extern int HashData(byte[] pbData, int cbData, byte[] piet, int outputLen);
    }
}
