using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SophiApp.Helpers
{
    internal class ProcessHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        internal static WindowsIdentity GetProcessUser(string process)
        {
            IntPtr processHandle = IntPtr.Zero;

            try
            {
                var proc = Process.GetProcessesByName(process).First();
                OpenProcessToken(proc.Handle, 8, out processHandle);
                return new WindowsIdentity(processHandle);
            }
            catch (Exception)
            {
                throw new Exception($"Cannot find the process called: {process}");
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                    CloseHandle(processHandle);
            }
        }

        internal static Process Start(string processName, string args, ProcessWindowStyle windowStyle)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = processName,
                Arguments = args,
                WindowStyle = windowStyle,
            });
        }

        internal static void StartWait(string processName, string args, ProcessWindowStyle windowStyle) => Process.Start(new ProcessStartInfo()
        {
            FileName = processName,
            Arguments = args,
            WindowStyle = windowStyle,
        }).WaitForExit();

        internal static void Stop(string processName)
        {
            var timeout = 10000;
            var procs = Process.GetProcessesByName(processName);

            foreach (var proc in procs)
            {
                proc.Kill();
                proc.WaitForExit(timeout);
                proc.Dispose();
            }
        }

        internal static void Stop(params string[] processNames)
        {
            foreach (var proc in processNames)
                Stop(proc);
        }
    }
}