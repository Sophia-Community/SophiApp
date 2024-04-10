// <copyright file="EnvironmentHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using SophiApp.Extensions;

    /// <summary>
    /// A class for working with environment variables.
    /// </summary>
    public static class EnvironmentHelper
    {
        private const int Msg = 273;
        private const int SMTOABORTIFHUNG = 0x0002;
        private const int WMSETTINGCHANGE = 0x1a;
        private const string TRAYSETTINGS = "TraySettings";

        // Virtual key ID of the F5 in File Explorer
        private static readonly IntPtr HWnd = new IntPtr(65535);
        private static readonly IntPtr HWNDBROADCAST = new IntPtr(0xffff);
        private static readonly UIntPtr UIntPtr = new UIntPtr(41504);

        /// <summary>
        /// Simulate pressing F5 to refresh the desktop.
        /// </summary>
        public static void RefreshUserDesktop() => _ = PostMessageW(HWnd, Msg, UIntPtr, IntPtr.Zero);

        /// <summary>
        /// Refresh desktop icons, environment variables, taskbar, etc.
        /// </summary>
        public static void ForcedRefresh()
        {
            _ = SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            _ = SendMessageTimeout(HWNDBROADCAST, WMSETTINGCHANGE, IntPtr.Zero, string.Empty, SMTOABORTIFHUNG, 100, IntPtr.Zero);
            _ = SendNotifyMessage(HWNDBROADCAST, WMSETTINGCHANGE, IntPtr.Zero, TRAYSETTINGS);
        }

        /// <summary>
        /// Stop the Start Menu process.
        /// </summary>
        public static void StopStartMenu() => Process.GetProcessesByName("StartMenuExperienceHost").ForEach(process => process.Close());

        /// <summary>
        /// Stop all explorer instances but only one in case enabled to launch folder windows in a separate process.
        /// </summary>
        public static void StopExplorerProcess()
        {
            Process.GetProcessesByName("explorer")
                .ForEach(process =>
                {
                    process.Close();
                    Thread.Sleep(1000);
                });
        }

        /// <summary>
        /// Simulate pressing F5 to refresh the desktop.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int PostMessageW(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Update desktop icons.
        /// </summary>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        /// <summary>
        /// Update environment variables.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int msg, IntPtr wParam, string lParam, int fuFlags, int uTimeout, IntPtr lpdwResult);

        /// <summary>
        /// Update taskbar.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern bool SendNotifyMessage(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);
    }
}
