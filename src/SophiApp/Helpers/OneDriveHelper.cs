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
        private const string ONE_DRIVE_SETUP_EXE = "OneDriveSetup.exe";
        private const string ONE_DRIVE_XML = "https://g.live.com/1rewlive5skydrive/OneDriveProductionV2";
        private const string ONEDRIVE_SETUP_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\OneDriveSetup.exe";
        private const string ONEDRIVE_UNINSTALL_MASK = "/uninstall";
        private const string ONEDRIVE_UNINSTALL_STRING = "UninstallString";
        private const string SOFTWARE_ONE_DRIVE = @"SOFTWARE\Microsoft\OneDrive";
        private const string SOFTWARE_WOW6432_ONE_DRIVE = @"SOFTWARE\WOW6432Node\Microsoft\OneDrive";
        private const string SYNC_SHELL64_DLL = "FileSyncShell64.dll";
        private const string USER_DOWNLOAD_FOLDER = "{374DE290-123F-4565-9164-39C4925E467B}";
        private const string USER_SHELL_FOLDER = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders";
        private static readonly string APPDATA_MS_ONE_DRIVE_FOLDER = $@"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\Microsoft\OneDrive";
        private static readonly string APPDATA_ONE_DRIVE_FOLDER = $@"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\OneDrive";
        private static readonly string ONE_DRIVE_FOLDER = Environment.GetEnvironmentVariable(ONE_DRIVE);
        private static readonly string ONE_DRIVE_LNK = $@"{Environment.GetEnvironmentVariable("APPDATA")}\Microsoft\Windows\Start Menu\Programs\OneDrive.lnk";
        private static readonly string ONE_DRIVE_TEMP = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\OneDriveTemp";
        private static readonly string PROGRAM_DATA_ONE_DRIVE = $@"{Environment.GetEnvironmentVariable("ProgramData")}\Microsoft OneDrive";
        internal static readonly string ONEDRIVE_SETUP_EXE = $@"{Environment.GetEnvironmentVariable("SystemRoot")}\SysWOW64\OneDriveSetup.exe";

        private static string GetUninstallString()
        {
            return RegHelper.GetStringValue(RegistryHive.CurrentUser, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING)
                    ?? RegHelper.GetStringValue(RegistryHive.LocalMachine, ONEDRIVE_SETUP_PATH, ONEDRIVE_UNINSTALL_STRING);
        }

        internal static bool HasSetupExe() => File.Exists(ONEDRIVE_SETUP_EXE);

        internal static void Install()
        {
            if (HasSetupExe())
            {
                ProcessHelper.StartWait(ONEDRIVE_SETUP_EXE);
                return;
            }

            var oneDriveUrl = WebHelper.GetXmlResponse(ONE_DRIVE_XML).DocumentElement.FirstChild.LastChild.Attributes[2].Value;
            var oneDriveFile = $@"{RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDER, USER_DOWNLOAD_FOLDER)}\{ONE_DRIVE_SETUP_EXE}";
            WebHelper.Download(url: oneDriveUrl, file: oneDriveFile, deleteIsExisting: true);
            ProcessHelper.StartWait(oneDriveFile);
            FileHelper.TryDeleteFile(oneDriveFile);
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
            FileHelper.DirectoryLazyDelete(PROGRAM_DATA_ONE_DRIVE);
            FileHelper.TryDeleteDirectory(ONE_DRIVE_TEMP);
            ScheduledTaskHelper.DeleteTask(ScheduledTaskHelper.FindAll(task => task.Name.Contains(ONE_DRIVE)));

            var oneDriveFolder = Directory.GetParent(uninstallString[0]).FullName;
            var syncShell64Dlls = Directory.GetFiles(oneDriveFolder, SYNC_SHELL64_DLL, SearchOption.AllDirectories);

            OsHelper.UnregisterDlls(syncShell64Dlls);
            FileHelper.DirectoryLazyDelete(oneDriveFolder, APPDATA_ONE_DRIVE_FOLDER, APPDATA_MS_ONE_DRIVE_FOLDER);
            FileHelper.TryDeleteFile(ONE_DRIVE_LNK);
        }
    }
}