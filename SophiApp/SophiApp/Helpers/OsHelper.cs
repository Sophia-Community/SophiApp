using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace SophiApp.Helpers
{
    internal class OsHelper
    {
        private const int Msg = 273;
        private const int SMTO_ABORTIFHUNG = 0x0002;
        private const int WM_SETTINGCHANGE = 0x1a;
        private static readonly IntPtr hWnd = new IntPtr(65535);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);

        // Virtual key ID of the F5 in File Explorer
        private static readonly UIntPtr UIntPtr = new UIntPtr(41504);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int PostMessageW(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, int fuFlags, int uTimeout, IntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        internal static string GetDisplayVersion() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData.CURRENT_VERSION, name: ActionsData.DISPLAY_VERSION_NAME);

        internal static string GetEdition() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData.CURRENT_VERSION, name: ActionsData.EDITION_ID_NAME);

        internal static string GetProductName() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData.CURRENT_VERSION, name: ActionsData.PRODUCT_NAME);

        internal static string GetRegisteredOrganization() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData.CURRENT_VERSION, name: ActionsData.REGISTRED_ORGANIZATION_NAME);

        internal static string GetRegisteredOwner() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData.CURRENT_VERSION, name: ActionsData.REGISTRED_OWNER_NAME);

        internal static bool IsEdition(string name) => GetEdition().Contains(name);

        public static void PostMessage() => PostMessageW(hWnd, Msg, UIntPtr, IntPtr.Zero);

        public static void Refresh()
        {
            // Update desktop icons
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            // Update environment variables
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, null, SMTO_ABORTIFHUNG, 100, IntPtr.Zero);
            // Update taskbar
            SendNotifyMessage(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, "TraySettings");
        }

        // Simulate pressing F5 to refresh the desktop
    }
}