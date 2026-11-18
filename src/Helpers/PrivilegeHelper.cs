// <copyright file="PrivilegeHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using System.Runtime.InteropServices;

/// <summary>
/// A helper for work with privileges of the app process.
/// </summary>
internal static class PrivilegeHelper
{
    private const uint TokenAdjustPrivileges = 0x0020;
    private const uint TokenQuery = 0x0008;
    private const int PrivilegeEnabled = 0x0002;
    private const int ErrorSuccess = 0;

    /// <summary>
    /// Turns on a privilege that an elevated process has, but that is turned off by default.
    /// </summary>
    /// <param name="name">The privilege name, for example "SeSecurityPrivilege".</param>
    /// <returns>Whether the privilege is turned on.</returns>
    public static bool TryEnable(string name)
    {
        if (!LookupPrivilegeValue(null, name, out var luid) || !OpenProcessToken(GetCurrentProcess(), TokenAdjustPrivileges | TokenQuery, out var token))
        {
            return false;
        }

        try
        {
            // TOKEN_PRIVILEGES with one privilege: the count, the identifier in two parts and the attributes
            int[] privileges = [1, (int)luid, (int)(luid >> 32), PrivilegeEnabled];

            // The call also succeeds when the process does not have the privilege at all, the error code tells these cases apart
            return AdjustTokenPrivileges(token, false, privileges, 0, nint.Zero, nint.Zero) && Marshal.GetLastWin32Error() == ErrorSuccess;
        }
        finally
        {
            _ = CloseHandle(token);
        }
    }

    [DllImport("kernel32.dll")]
    private static extern nint GetCurrentProcess();

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(nint handle);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenProcessToken(nint process, uint desiredAccess, out nint token);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool LookupPrivilegeValue(string? systemName, string name, out long luid);

    [DllImport("advapi32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AdjustTokenPrivileges(nint token, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, int[] newState, int bufferLength, nint previousState, nint returnLength);
}
