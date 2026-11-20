// <copyright file="StartMenuHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Diagnostics;

    /// <summary>
    /// Looks for a third-party Start menu: Start11, StartAllBack or Open-Shell.
    /// </summary>
    public static class StartMenuHelper
    {
        private static readonly Lock CheckLock = new();
        private static bool? thirdPartyMenuExists;

        /// <summary>
        /// Determines whether a third-party Start menu is running. The answer is remembered until <see cref="Reset"/> is called.
        /// </summary>
        public static bool ThirdPartyMenuExists()
        {
            lock (CheckLock)
            {
                return thirdPartyMenuExists ??= Detect();
            }
        }

        /// <summary>
        /// Forgets the answer, so the next call looks for the Start menu again.
        /// </summary>
        public static void Reset()
        {
            lock (CheckLock)
            {
                thirdPartyMenuExists = null;
            }
        }

        private static bool Detect()
        {
            var processes = Process.GetProcesses();

            try
            {
                if (Array.Exists(processes, process => IsNamed(process, "Start11Srv") || IsNamed(process, "StartMenu")))
                {
                    return true;
                }

                // StartAllBack has no process of its own: it is loaded into explorer.exe as a library
                return processes.Where(process => IsNamed(process, "explorer")).Any(HasStartAllBack);
            }
            finally
            {
                Array.ForEach(processes, process => process.Dispose());
            }
        }

        private static bool IsNamed(Process process, string name) => process.ProcessName.Equals(name, StringComparison.OrdinalIgnoreCase);

        private static bool HasStartAllBack(Process explorer)
        {
            try
            {
                return explorer.Modules.Cast<ProcessModule>().Any(module => module.ModuleName.StartsWith("StartAllBack", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                // The process has exited or its modules cannot be read
                App.Logger.LogSkippedException("Reading the modules of File Explorer", ex);
                return false;
            }
        }
    }
}
