// <copyright file="ForegroundHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Keeps the app window usable after File Explorer is restarted.
    /// </summary>
    public static class ForegroundHelper
    {
        private const string SearchProcessName = "SearchHost";
        private const string SearchWindowClass = "Windows.UI.Core.CoreWindow";
        private const byte EscapeKey = 0x1B;
        private const uint KeyUp = 0x0002;

        // DWMWA_CLOAKED: a window that is shown to the system but not to the user
        private const int CloakedAttribute = 14;

        // Windows Search comes up within a couple of seconds after File Explorer
        private static readonly TimeSpan WatchTime = TimeSpan.FromSeconds(8);
        private static readonly TimeSpan WatchStep = TimeSpan.FromMilliseconds(150);

        private delegate bool EnumWindowsCallback(nint window, nint parameter);

        /// <summary>
        /// Gets the foreground processes that were seen in turn while the search window was watched for. It goes to the log.
        /// </summary>
        public static string LastWatch { get; private set; } = string.Empty;

        /// <summary>
        /// Closes the window of Windows Search if it comes up after File Explorer is restarted.
        /// Windows starts Search again together with File Explorer, and its window may come up invisible over the middle of the screen:
        /// the clicks on the app window under it are lost, and the window cannot be moved until the user switches to another one.
        /// </summary>
        /// <param name="appWindow">The handle of the app window, which gets the foreground back.</param>
        /// <returns>True if the search window was closed.</returns>
        public static async Task<bool> CloseSearchAsync(nint appWindow)
        {
            var timer = Stopwatch.StartNew();
            var seen = new List<string>();

            while (timer.Elapsed < WatchTime)
            {
                await Task.Delay(WatchStep);
                var search = FindShownSearchWindow();
                var state = $"{GetProcessName(GetForegroundWindow())}{(search != 0 ? "+search" : string.Empty)}";

                if (seen.Count == 0 || seen[^1] != state)
                {
                    seen.Add(state);
                }

                if (search == 0)
                {
                    continue;
                }

                // Esc goes to the foreground window, so the search window has to be the one
                if (GetForegroundWindow() != search)
                {
                    _ = SetForegroundWindow(search);
                    await Task.Delay(WatchStep);
                }

                if (GetForegroundWindow() == search)
                {
                    // Search closes itself the same way it does for the user
                    keybd_event(EscapeKey, 0, 0, 0);
                    keybd_event(EscapeKey, 0, KeyUp, 0);
                    await Task.Delay(WatchStep);
                    _ = SetForegroundWindow(appWindow);
                    LastWatch = $"{string.Join(" > ", seen)} > closed at {timer.Elapsed.TotalSeconds:F1} s";
                    return true;
                }
            }

            LastWatch = string.Join(" > ", seen);
            return false;
        }

        private static nint FindShownSearchWindow()
        {
            nint found = 0;

            _ = EnumWindows(
                (window, _) =>
                {
                    if (IsWindowVisible(window) && IsShownSearchWindow(window))
                    {
                        found = window;
                        return false;
                    }

                    return true;
                },
                0);

            return found;
        }

        private static bool IsShownSearchWindow(nint window)
        {
            var className = new StringBuilder(64);
            _ = GetClassName(window, className, className.Capacity);

            return className.ToString() == SearchWindowClass
                && DwmGetWindowAttribute(window, CloakedAttribute, out var cloaked, sizeof(int)) == 0
                && cloaked == 0
                && GetProcessName(window).Equals(SearchProcessName, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetProcessName(nint window)
        {
            if (window == 0 || GetWindowThreadProcessId(window, out var processId) == 0)
            {
                return string.Empty;
            }

            try
            {
                using var process = Process.GetProcessById((int)processId);
                return process.ProcessName;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                // The process has already exited
                return string.Empty;
            }
        }

        [DllImport("user32.dll")]
        private static extern nint GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(nint window);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(nint window, out uint processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsCallback callback, nint parameter);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(nint window);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(nint window, StringBuilder className, int maxCount);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(nint window, int attribute, out int value, int size);

        #pragma warning disable SA1300 // The name of the Windows function
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte key, byte scanCode, uint flags, nuint extraInfo);
        #pragma warning restore SA1300
    }
}
