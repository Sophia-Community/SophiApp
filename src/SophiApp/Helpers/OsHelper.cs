using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class OsHelper
    {
        private const string AUTORESTART_SHELL = "AutoRestartShell";
        private const string CURRENT_BUILD = "CurrentBuild";
        private const string CURRENT_VERSION = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        private const byte DISABLED_VALUE = 0;
        private const string DISPLAY_VERSION_NAME = "DisplayVersion";
        private const string EDITION_ID_NAME = "EditionID";
        private const byte ENABLED_VALUE = 1;
        private const string EXPLORER = "explorer";
        private const string MAJOR_VERSION_NUMBER = "CurrentMajorVersionNumber";
        private const string MINOR_VERSION_NUMBER = "CurrentMinorVersionNumber";
        private const int Msg = 273;
        private const string PRODUCT_NAME = "ProductName";
        private const string REGISTRED_ORGANIZATION_NAME = "RegisteredOrganization";
        private const string REGISTRED_OWNER_NAME = "RegisteredOwner";
        private const string REGSVR_32 = "regsvr32.exe";
        private const int SMTO_ABORTIFHUNG = 0x0002;
        private const string START_MENU_PROCESS = "StartMenuExperienceHost";
        private const int TIMEOUT_3_SECONDS = 3000;
        private const string TRAY_SETTINGS = "TraySettings";
        private const string UBR = "UBR";
        private const string WIN_ENTERPRISE_G = "EnterpriseG";
        private const uint WIN11_ORIGINAL_BUILD = 22;
        private const uint WIN11_INSIDER_BUILD_23 = 23;
        private const uint WIN11_INSIDER_BUILD_25 = 25;
        private const string WINLOGON_PATH = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";
        private const int WM_SETTINGCHANGE = 0x1a;
        private static readonly IntPtr hWnd = new IntPtr(65535);
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private static readonly string REGISTRY_CURRENT_VERSION = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        // Virtual key ID of the F5 in File Explorer
        private static readonly UIntPtr UIntPtr = new UIntPtr(41504);

        internal const uint WIN10_MIN_SUPPORTED_BUILD = 19045;
        internal const uint WIN10_MIN_SUPPORTED_UBR = 3208;
        internal const uint WIN11_INSIDER_BUILD_PATTERN = 22500;
        internal const uint WIN11_INSIDER_BUILD = 22621;
        internal const uint WIN11_MIN_SUPPORTED_BUILD = 22000;
        internal const uint WIN11_MIN_SUPPORTED_UBR = 1992;

        internal static bool IsEnterpriseG = GetEdition() == WIN_ENTERPRISE_G;

        private static WindowsIdentity GetCurrentUser() => WindowsIdentity.GetCurrent();

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

        internal static string GetProductName()
        {
            var productName = RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: PRODUCT_NAME) as string;
            return IsWindows11() ? productName.Replace("0", "1") : productName;
        }

        internal static string GetRegionName() => RegionInfo.CurrentRegion.EnglishName;

        internal static string GetRegisteredOrganization() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: REGISTRED_ORGANIZATION_NAME) as string;

        internal static string GetRegisteredOwner() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: CURRENT_VERSION, name: REGISTRED_OWNER_NAME) as string;

        internal static ushort GetUpdateBuildRevision() => Convert.ToUInt16(RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, UBR));

        internal static Version GetVersion()
        {
            var major = RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, MAJOR_VERSION_NUMBER).ToInt32();
            var minor = RegHelper.GetValue(hive: RegistryHive.LocalMachine, REGISTRY_CURRENT_VERSION, MINOR_VERSION_NUMBER).ToInt32();
            var build = GetBuild();
            var revision = GetUpdateBuildRevision();
            return new Version(major, minor, build, revision);
        }

        internal static bool IsEdition(string name) => GetEdition().Contains(name);

        internal static bool IsWindows11()
        {
            var build = GetBuild() / 1000;
            return build == WIN11_ORIGINAL_BUILD || build == WIN11_INSIDER_BUILD_23 || build == WIN11_INSIDER_BUILD_25;
        }

        internal static void SafelyRestartExplorerProcess()
        {
            // Save opened folders
            var openedFolders = ComObjectHelper.GetOpenedFolders().ToList();
            // Terminate the File Explorer process
            RegHelper.SetValue(RegistryHive.LocalMachine, WINLOGON_PATH, AUTORESTART_SHELL, DISABLED_VALUE, RegistryValueKind.DWord);
            ProcessHelper.Stop(EXPLORER);
            Task.Delay(TIMEOUT_3_SECONDS).Wait();
            RegHelper.SetValue(RegistryHive.LocalMachine, WINLOGON_PATH, AUTORESTART_SHELL, ENABLED_VALUE, RegistryValueKind.DWord);
            // Start the File Explorer process
            _ = ProcessHelper.Start(EXPLORER);
            Task.Delay(TIMEOUT_3_SECONDS).Wait();
            // Restoring closed folders
            ProcessHelper.StartWait(EXPLORER, openedFolders, ProcessWindowStyle.Minimized);
            Task.Delay(TIMEOUT_3_SECONDS).Wait();
        }

        internal static void SetRecommendedTroubleshooting(byte autoOrDefault)
        {
            // RecommendedTroubleshooting
            // https://github.com/farag2/Sophia-Script-for-Windows/blob/master/Sophia%20Script/Sophia%20Script%20for%20Windows%2010/Module/Sophia.psm1#L7158
            // 3 - Automatically
            // 2 - Default

            const string WINDOWS_MITIGATION_PATH = @"SOFTWARE\Microsoft\WindowsMitigation";
            const string DATA_COLLECTION_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";
            const string DIAG_TRACK_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack";
            const string ERROR_REPORTING_PATH = @"SOFTWARE\Microsoft\Windows\Windows Error Reporting";
            const string DISABLE_ERROR_REPORTING = "Disabled";
            const string MITIGATION_USER_PREFERENCE = "UserPreference";
            const string SHOWED_TOAST_LEVEL = "ShowedToastAtLevel";
            const string ALLOW_TELEMETRY = "AllowTelemetry";
            const string MAX_TELEMETRY = "MaxTelemetryAllowed";
            const string QUEUE_REPORTING_TASK = "QueueReporting";
            const string QUEUE_TASK_PATH = @"Microsoft\Windows\Windows Error Reporting";
            const string ERROR_REPORTING_SERVICE = "WerSvc";
            const byte AUTOMATICALLY_VALUE = 3;

            RegHelper.SetValue(RegistryHive.LocalMachine, WINDOWS_MITIGATION_PATH, MITIGATION_USER_PREFERENCE, autoOrDefault, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, ALLOW_TELEMETRY, AUTOMATICALLY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, MAX_TELEMETRY, AUTOMATICALLY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL, AUTOMATICALLY_VALUE, RegistryValueKind.DWord);
            ScheduledTaskHelper.TryChangeTaskState(taskPath: QUEUE_TASK_PATH, taskName: QUEUE_REPORTING_TASK, enable: true);
            RegHelper.DeleteKey(RegistryHive.CurrentUser, ERROR_REPORTING_PATH, DISABLE_ERROR_REPORTING);

            var werSvc = ServiceHelper.Get(ERROR_REPORTING_SERVICE);
            ServiceHelper.SetStartMode(werSvc, System.ServiceProcess.ServiceStartMode.Manual);
            werSvc.Start();
        }

        internal static void UnregisterDlls(string[] syncShell64Dlls)
        {
            foreach (var dll in syncShell64Dlls)
                ProcessHelper.StartWait(REGSVR_32, $"/u /s {dll}");
        }

        public static void PostMessage() => PostMessageW(hWnd, Msg, UIntPtr, IntPtr.Zero);

        public static void RefreshEnvironment()
        {
            // Update Desktop Icons
            SHChangeNotify(0x8000000, 0x1000, IntPtr.Zero, IntPtr.Zero);
            // Update Environment Variables
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, null, SMTO_ABORTIFHUNG, 100, IntPtr.Zero);
            // Update Taskbar
            SendNotifyMessage(HWND_BROADCAST, WM_SETTINGCHANGE, IntPtr.Zero, TRAY_SETTINGS);
            // Update Start Menu
            ProcessHelper.Stop(START_MENU_PROCESS);
        }
    }
}