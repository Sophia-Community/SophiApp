// <copyright file="EnvironmentHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helper class for working with environment variables.
    /// </summary>
    public static class EnvironmentHelper
    {
        private const int Msg = 273;
        private const int SMTOABORTIFHUNG = 0x0002;
        private const int WMSETTINGCHANGE = 0x1a;
        private const string TRAYSETTINGS = "TraySettings";

        private static readonly nint HWnd = 65535;
        private static readonly nint HWNDBROADCAST = 0xffff;

        // Virtual key ID of the F5 in File Explorer
        private static readonly nuint RefreshKey = 41504;

        /// <summary>
        /// Gets the folder of the app executable. The log and the settings are kept there.
        /// </summary>
        public static string AppFolder { get; } = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;

        /// <summary>
        /// Simulate pressing F5 to refresh the desktop.
        /// </summary>
        public static void RefreshUserDesktop()
        {
            PostMessageW(HWnd, Msg, RefreshKey, nint.Zero);
        }

        /// <summary>
        /// Refresh desktop icons, environment variables, taskbar, etc.
        /// </summary>
        public static void ForcedRefresh()
        {
            SHChangeNotify(0x8000000, 0x1000, nint.Zero, nint.Zero);
            SendMessageTimeout(HWNDBROADCAST, WMSETTINGCHANGE, nint.Zero, "Environment", SMTOABORTIFHUNG, 100, nint.Zero);
            SendNotifyMessage(HWNDBROADCAST, WMSETTINGCHANGE, nint.Zero, TRAYSETTINGS);
        }

        /// <summary>
        /// Simulate pressing F5 to refresh the desktop.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int PostMessageW(nint hWnd, uint msg, nuint wParam, nint lParam);

        /// <summary>
        /// Update desktop icons.
        /// </summary>
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int SHChangeNotify(int eventId, int flags, nint item1, nint item2);

        /// <summary>
        /// Update environment variables.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern nint SendMessageTimeout(nint hWnd, int msg, nint wParam, string? lParam, int fuFlags, int uTimeout, nint lpdwResult);

        /// <summary>
        /// Update taskbar.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern bool SendNotifyMessage(nint hWnd, uint msg, nint wParam, string lParam);
    }
}
