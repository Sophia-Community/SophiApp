using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace SophiApp.Helpers
{
    internal class ServiceHelper
    {
        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;

        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;

        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;

        private const uint SERVICE_QUERY_CONFIG = 0x00000001;

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ChangeServiceConfig(
        IntPtr hService,
        UInt32 nServiceType,
        UInt32 nStartType,
        UInt32 nErrorControl,
        String lpBinaryPathName,
        String lpLoadOrderGroup,
        IntPtr lpdwTagId,
        [In] char[] lpDependencies,
        String lpServiceStartName,
        String lpPassword,
        String lpDisplayName);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        private static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        internal static ServiceController Get(string serviceName) => new ServiceController(serviceName);

        internal static void Restart(string serviceName)
        {
            var timeout = 10.0;
            var service = new ServiceController(serviceName);
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(timeout));
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(timeout));
        }

        internal static bool ServiceExist(string serviceName) => ServiceController.GetServices().Any(service => service.ServiceName == serviceName);

        internal static void TryRestart(string serviceName)
        {
            if (Get(serviceName).StartType != ServiceStartMode.Disabled)
            {
                Restart(serviceName);
            }
        }

        public static void SetStartMode(ServiceController svc, ServiceStartMode mode)
        {
            var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (scManagerHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Manager Error");
            }

            var serviceHandle = OpenService(
                scManagerHandle,
                svc.ServiceName,
                SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

            if (serviceHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Error");
            }

            var result = ChangeServiceConfig(
                serviceHandle,
                SERVICE_NO_CHANGE,
                (uint)mode,
                SERVICE_NO_CHANGE,
                null,
                null,
                IntPtr.Zero,
                null,
                null,
                null,
                null);

            if (result == false)
            {
                int nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);
                throw new ExternalException("Could not change service start type: "
                    + win32Exception.Message);
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }
    }
}