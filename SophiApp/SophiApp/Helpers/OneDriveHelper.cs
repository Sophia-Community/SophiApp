using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SophiApp.Helpers
{
    internal class OneDriveHelper
    {
        private const string ENVIRONMENT = "Environment";
        private const string ONE_DRIVE = "OneDrive";
        private const string ONE_DRIVE_AUTH = "FileCoAuth";
        private const string ONE_DRIVE_CONSUMER = "OneDriveConsumer";
        private const string ONE_DRIVE_SETUP = "OneDriveSetup";
        private const string ONEDRIVE_SETUP_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\OneDriveSetup.exe";
        private const string ONEDRIVE_UNINSTALL_MASK = "/uninstall";
        private const string ONEDRIVE_UNINSTALL_STRING = "UninstallString";
        private const string SOFTWARE_ONE_DRIVE = @"SOFTWARE\Microsoft\OneDrive";
        private const string SOFTWARE_WOW6432_ONE_DRIVE = @"SOFTWARE\WOW6432Node\Microsoft\OneDrive";
        private const string SYNC_SHELL64_DLL = "FileSyncShell64.dll";
        private static readonly string APPDATA_MS_ONE_DRIVE_FOLDER = $@"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\Microsoft\OneDrive";
        private static readonly string APPDATA_ONE_DRIVE_FOLDER = $@"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\OneDrive";
        private static readonly string ONE_DRIVE_FOLDER = Environment.GetEnvironmentVariable(ONE_DRIVE);
        private static readonly string ONE_DRIVE_LNK = $@"{Environment.GetEnvironmentVariable("APPDATA")}\Microsoft\Windows\Start Menu\Programs\OneDrive.lnk";
        private static readonly string ONE_DRIVE_TEMP = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\OneDriveTemp";
        private static readonly string PROGRAM_DATA_ONE_DRIVE = $@"{Environment.GetEnvironmentVariable("ProgramData")}\Microsoft OneDrive";

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
            ProcessHelper.StartWait(uninstallString[0], uninstallString.Length == 2 ? uninstallString[1] : $"{uninstallString[1]} {uninstallString[2]}");

            if (Directory.Exists(ONE_DRIVE_FOLDER) && FileHelper.DirectoryIsEmpty(ONE_DRIVE_FOLDER))
                FileHelper.DirectoryLazyDelete(ONE_DRIVE_FOLDER);

            RegHelper.TryDeleteKey(RegistryHive.CurrentUser, ENVIRONMENT, ONE_DRIVE, ONE_DRIVE_CONSUMER);
            RegHelper.TryDeleteSubKeyTree(RegistryHive.CurrentUser, SOFTWARE_ONE_DRIVE);
            RegHelper.TryDeleteSubKeyTree(RegistryHive.LocalMachine, SOFTWARE_WOW6432_ONE_DRIVE);
            FileHelper.TryDeleteDirectory(PROGRAM_DATA_ONE_DRIVE);
            FileHelper.TryDeleteDirectory(ONE_DRIVE_TEMP);
            ScheduledTaskHelper.Delete(ScheduledTaskHelper.FindAll(task => task.Name.Contains(ONE_DRIVE)));

            var oneDriveFolder = Directory.GetParent(uninstallString[0]).FullName;
            var syncShell64Dlls = Directory.GetFiles(oneDriveFolder, SYNC_SHELL64_DLL, SearchOption.AllDirectories);

            OsHelper.UnregisterDlls(syncShell64Dlls);
            FileHelper.DirectoryLazyDelete(oneDriveFolder, APPDATA_ONE_DRIVE_FOLDER, APPDATA_MS_ONE_DRIVE_FOLDER);
            FileHelper.FileDelete(ONE_DRIVE_LNK);
        }
    }
}