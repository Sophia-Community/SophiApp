// <copyright file="Program.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;

using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Windows.AppLifecycle;
using SophiApp.Helpers;

/// <summary>
/// The entry point of the app. Only one copy of the app runs: a second one shows the window of the first and exits.
/// </summary>
public static class Program
{
    private const string InstanceKey = "2e340960-5e58-4e2d-b0c1-0a1b54145345";
    private const int RestoreWindow = 9;

    [STAThread]
    private static void Main()
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();
        var firstCopy = AppInstance.FindOrRegisterForKey(InstanceKey);

        // The check is made before anything of the app is created, so the second copy has nothing to close
        if (!firstCopy.IsCurrent)
        {
            ShowWindowOf(firstCopy.ProcessId);
            return;
        }

        // Only the first copy cleans up after itself
        SelfExtractHelper.RemoveOnExit();

        XamlGeneratedProgram.XamlGeneratedMain();
    }

    private static void ShowWindowOf(uint processId)
    {
        try
        {
            using var process = Process.GetProcessById((int)processId);
            var window = process.MainWindowHandle;

            if (window == 0)
            {
                return;
            }

            if (IsIconic(window))
            {
                _ = ShowWindow(window, RestoreWindow);
            }

            _ = SetForegroundWindow(window);
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            // The first copy has just exited
        }
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsIconic(nint window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(nint window, int command);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(nint window);
}
