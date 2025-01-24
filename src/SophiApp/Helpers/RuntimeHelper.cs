// <copyright file="RuntimeHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Implements the logic app runtime helper API.
/// </summary>
public static class RuntimeHelper
{
    /// <summary>
    /// Gets a value indicating whether MSIX is used.
    /// </summary>
    public static bool IsMSIX
    {
        get
        {
            var length = 0;
            return GetCurrentPackageFullName(ref length, null) != 15700L;
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder? packageFullName);
}
