using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;

namespace SophiApp.Helpers
{
    internal class OneDriveHelper
    {
        private const string ONEDRIVE_SETUP_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\OneDriveSetup.exe";
        private const string ONEDRIVE_UNINSTALL_MASK = "/uninstall";
        private const string ONEDRIVE_UNINSTALL_STRING = "UninstallString";
        internal const string ONE_DRIVE = "OneDrive";
        internal const string ONE_DRIVE_AUTH = "FileCoAuth";
        internal const string ONE_DRIVE_SETUP = "OneDriveSetup";

        private static string GetUninstallString()
        {
            return RegHelper.GetStringValue(RegistryHive.CurrentUser, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING)
                    ?? RegHelper.GetStringValue(RegistryHive.LocalMachine, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING);
        }

        internal static bool IsInstalled()
        {
            return (RegHelper.KeyExist(RegistryHive.CurrentUser, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING)
                        && RegHelper.GetStringValue(RegistryHive.CurrentUser, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING).Contains(ONEDRIVE_UNINSTALL_MASK))
                        || (RegHelper.KeyExist(RegistryHive.LocalMachine, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING)
                            && RegHelper.GetStringValue(RegistryHive.LocalMachine, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING).Contains(ONEDRIVE_UNINSTALL_MASK));
        }

        internal static void StopProcesses()
        {
            ProcessHelper.Stop(ONE_DRIVE, ONE_DRIVE_SETUP, ONE_DRIVE_AUTH);
        }

        internal static void Uninstall()
        {
            var uninstallString = Regex.Replace(GetUninstallString(), @"\s*/", @",/").Split(',');
            Array.ForEach(uninstallString, str => str.Trim());
            StopProcesses();
            ProcessHelper.StartWait(uninstallString[0], uninstallString.Length == 2 ? uninstallString[1]
                                                                                    : $"{uninstallString[1]} {uninstallString[2]}");
        }
    }
}