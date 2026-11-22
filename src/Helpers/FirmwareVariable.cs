// <copyright file="FirmwareVariable.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Reads the variables of the UEFI firmware, such as the state of Secure Boot and its signature database.
    /// </summary>
    public static class FirmwareVariable
    {
        /// <summary>
        /// The namespace of the variables that the UEFI specification defines, such as "SecureBoot".
        /// </summary>
        public const string GlobalNamespace = "{8BE4DF61-93CA-11D2-AA0D-00E098032B8C}";

        /// <summary>
        /// The namespace of the Secure Boot signature databases, such as "db".
        /// </summary>
        public const string ImageSecurityNamespace = "{D719B2CB-3D3A-4596-A3BC-DAD00E67656F}";

        private const string SystemEnvironmentPrivilege = "SeSystemEnvironmentPrivilege";
        private const uint AdjustPrivilegesAndQuery = 0x0020 | 0x0008;
        private const uint PrivilegeEnabled = 0x00000002;
        private const int NotAllAssigned = 1300;
        private const int InsufficientBuffer = 122;
        private const int FirstBufferSize = 16 * 1024;
        private const int LargestBufferSize = 1024 * 1024;

        /// <summary>
        /// Reads a firmware variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="namespaceGuid">The namespace of the variable.</param>
        /// <returns>The value, or null if the computer has no such variable, is not a UEFI one, or the app may not read it.</returns>
        public static byte[]? Read(string name, string namespaceGuid)
        {
            // Windows gives the variables only to a process that has turned this privilege on. Administrators have it turned off
            if (!EnablePrivilege(SystemEnvironmentPrivilege))
            {
                return null;
            }

            for (var size = FirstBufferSize; size <= LargestBufferSize; size *= 4)
            {
                var buffer = new byte[size];
                var read = GetFirmwareEnvironmentVariable(name, namespaceGuid, buffer, (uint)buffer.Length);

                if (read > 0)
                {
                    return buffer[..(int)read];
                }

                if (Marshal.GetLastWin32Error() != InsufficientBuffer)
                {
                    return null;
                }
            }

            return null;
        }

        private static bool EnablePrivilege(string privilege)
        {
            if (!OpenProcessToken(GetCurrentProcess(), AdjustPrivilegesAndQuery, out var token))
            {
                return false;
            }

            try
            {
                var privileges = new TokenPrivileges { Count = 1, Attributes = PrivilegeEnabled };

                return LookupPrivilegeValue(null, privilege, out privileges.Luid)
                    && AdjustTokenPrivileges(token, false, ref privileges, 0, 0, 0)
                    && Marshal.GetLastWin32Error() != NotAllAssigned;
            }
            finally
            {
                _ = CloseHandle(token);
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "GetFirmwareEnvironmentVariableW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetFirmwareEnvironmentVariable(string name, string namespaceGuid, byte[] buffer, uint size);

        [DllImport("kernel32.dll")]
        private static extern nint GetCurrentProcess();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(nint handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(nint process, uint access, out nint token);

        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueW", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LookupPrivilegeValue(string? systemName, string name, out long luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(nint token, [MarshalAs(UnmanagedType.Bool)] bool disableAll, ref TokenPrivileges newState, uint bufferLength, nint previousState, nint returnLength);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct TokenPrivileges
        {
            public uint Count;
            public long Luid;
            public uint Attributes;
        }
    }
}
