using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SophiApp.Helpers
{
    internal class ProcessHelper
    {
        internal static WindowsIdentity GetProcessUser(string process)
        {
            IntPtr processHandle = IntPtr.Zero;

            try
            {
                var proc = Process.GetProcessesByName(process).First();
                OpenProcessToken(proc.Handle, 8, out processHandle);
                return new WindowsIdentity(processHandle);
            }
            catch (Exception e)
            {
                throw new Exception($"Cannot find the process called: {process}");
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                    CloseHandle(processHandle);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
    }
}