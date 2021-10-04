using Microsoft.Win32;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace SophiApp.Helpers
{
    internal class OsHelper
    {
        private const string CURRENT_BUILD = "CurrentBuild";
        private const string CURRENT_VERSION = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        private const string DISPLAY_VERSION_NAME = "DisplayVersion";
        private const string EDITION_ID_NAME = "EditionID";
        private const string MAJOR_VERSION_NUMBER = "CurrentMajorVersionNumber";
        private const string MINOR_VERSION_NUMBER = "CurrentMinorVersionNumber";
        private const int Msg = 273;
        private const string PRODUCT_NAME = "ProductName";
        private const string REGISTRED_ORGANIZATION_NAME = "RegisteredOrganization";
        private const string REGISTRED_OWNER_NAME = "RegisteredOwner";
        private const int SMTO_ABORTIFHUNG = 0x0002;
        private const string TRAY_SETTINGS = "TraySettings";
        private const string UBR = "UBR";
        private const int WM_SETTINGCHANGE = 0x1a;
        private static readonly IntPtr hWnd = new IntPtr(65535);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private static readonly string REGISTRY_CURRENT_VERSION = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        // Virtual key ID of the F5 in File Explorer
        private static readonly UIntPtr UIntPtr = new UIntPtr(41504);

        private static WindowsIdentity GetCurrentUser() => System.Security.Principal.WindowsIdentity.GetCurrent();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int PostMessageW(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, string lParam, int fuFlags, int uTimeout, IntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        internal static ushort GetBuild() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, CURRENT_BUILD).ToUshort();

        internal static string GetCurrentCultureName() => CultureInfo.CurrentCulture.EnglishName;

        internal static SecurityIdentifier GetCurrentUserSid() => GetCurrentUser().User;

        internal static string GetDisplayVersion() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: DISPLAY_VERSION_NAME) as string;

        internal static string GetEdition() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: EDITION_ID_NAME) as string;

        internal static string GetProductName() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: PRODUCT_NAME) as string;

        internal static string GetRegionName() => RegionInfo.CurrentRegion.EnglishName;

        internal static string GetRegisteredOrganization() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: REGISTRED_ORGANIZATION_NAME) as string;

        internal static string GetRegisteredOwner() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: REGISTRED_OWNER_NAME) as string;

        internal static ushort GetUpdateBuildRevision() => Convert.ToUInt16(RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, UBR));

        internal static string GetVersion()
        {
            var majorVersion = RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, MAJOR_VERSION_NUMBER);
            var minorVersion = RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, MINOR_VERSION_NUMBER);
            var buildVersion = GetBuild();
            var ubrVersion = GetUpdateBuildRevision();
            return $"{majorVersion}.{minorVersion}.{buildVersion}.{ubrVersion}";
        }

        internal static bool IsEdition(string name) => GetEdition().Contains(name);

        public static void PostMessage() => PostMessageW(hWnd, Msg, UIntPtr, IntPtr.Zero);

        public static void Refresh()
        {
            // Update desktop icons
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            // Update environment variables
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, null, SMTO_ABORTIFHUNG, 100, IntPtr.Zero);
            // Update taskbar
            SendNotifyMessage(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, TRAY_SETTINGS);
        }
    }
}