using System;
using System.Collections.Generic;
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

        internal static IEnumerable<WindowsIdentity> GetProcessIdentity(string process)
        {
            IntPtr processHandle = IntPtr.Zero;

            foreach (var proc in Process.GetProcessesByName(process))
            {
                try
                {
                    OpenProcessToken(proc.Handle, 8, out processHandle);
                    yield return new WindowsIdentity(processHandle);
                }
                finally
                {
                    if (processHandle != IntPtr.Zero)
                        CloseHandle(processHandle);
                }
            }
        }

        internal static bool ProcessExist(string processName) => Process.GetProcessesByName(processName).Count() > 0;

        internal static Process Start(string processName, string args = null, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = processName,
                Arguments = args,
                WindowStyle = windowStyle,
            });
        }

        internal static void StartWait(string processName, string args = null, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal) => Process.Start(new ProcessStartInfo()
        {
            FileName = processName,
            Arguments = args,
            WindowStyle = windowStyle,
        }).WaitForExit();

        internal static void StartWait(string processName, List<string> args, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            foreach (var path in args)
            {
                StartWait(processName, path, windowStyle);
            }
        }

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