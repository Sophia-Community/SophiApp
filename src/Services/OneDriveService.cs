// <copyright file="OneDriveService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System.Diagnostics;
    using static System.Environment;

    /// <inheritdoc/>
    public class OneDriveService : IOneDriveService
    {
        private readonly IHttpService httpService;
        private readonly IProcessService processService;
        private readonly IScheduledTaskService scheduledTaskService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveService"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="processService">A service for working with Windows <see cref="Process"/> API.</param>
        /// <param name="scheduledTaskService">A service for working with Scheduled Task API.</param>
        public OneDriveService(
            IHttpService httpService,
            IProcessService processService,
            IScheduledTaskService scheduledTaskService)
        {
            this.httpService = httpService;
            this.processService = processService;
            this.scheduledTaskService = scheduledTaskService;
        }

        /// <inheritdoc/>
        public string GetSetupFileOrDefault()
        {
            var setupFile = Path.Combine(SystemDirectory, "OneDriveSetup.exe");
            return File.Exists(setupFile) ? setupFile : string.Empty;
        }

        /// <inheritdoc/>
        public void Install(bool allUsers)
        {
            var setupFile = GetSetupFileOrDefault();
            var arguments = allUsers ? "/allusers" : string.Empty;

            if (!string.IsNullOrEmpty(setupFile))
            {
                App.Status.SetStep("Panel_Step_Installing");
                processService.WaitForAllExit(setupFile, arguments);
            }
            else
            {
                var downloadPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders";
                // Get user Download folder path
                var downloadFolder = Registry.CurrentUser.GetSubKeyValue(downloadPath, "{374DE290-123F-4565-9164-39C4925E467B}") as string ?? string.Empty;
                var downloadFile = Path.Combine(downloadFolder, "OneDriveSetup.exe");
                httpService.DownloadOneDrive(downloadFile);
                App.Status.SetStep("Panel_Step_Installing");
                processService.WaitForAllExit(downloadFile, arguments);
                Thread.Sleep(3000);

                // The installer may still be running
                processService.KillProcessByName("OneDriveSetup", 3000);
                downloadFile.TryDelete();
            }

            // Save screenshots in the Pictures folder when pressing Windows+PrtScr or using Windows+Shift+S
            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "{B7BEDE81-DF94-4682-A7D8-57A52620B86F}");
            scheduledTaskService.StartOneDriveUpdateTasks();
        }

        /// <inheritdoc/>
        public bool IsInstalled() => !string.IsNullOrEmpty(GetUninstallString());

        /// <inheritdoc/>
        public void Uninstall()
        {
            var uninstallString = GetUninstallString().Trim();

            if (string.IsNullOrEmpty(uninstallString))
            {
                return;
            }

            // Getting the path to OneDriveSetup.exe and its arguments. The path may be quoted or not
            var fileEnd = uninstallString.StartsWith('"') ? uninstallString.IndexOf('"', 1) : uninstallString.IndexOf(".exe", StringComparison.OrdinalIgnoreCase) + 4;
            var uninstallFile = uninstallString[..fileEnd].Trim('"');
            var uninstallArgs = uninstallString[fileEnd..].Trim('"', ' ');

            // The OneDrive folder is the parent of the version folder with OneDriveSetup.exe
            var oneDriveFolder = Path.GetDirectoryName(Path.GetDirectoryName(uninstallFile)) ?? string.Empty;
            var userDataFolder = GetUserDataFolderOrDefault();
            App.Status.SetStep("Panel_Step_Uninstalling");
            processService.KillProcessByName(3000, "OneDrive", "OneDriveSetup", "FileCoAuth");

            using (processService.WaitForExit(uninstallFile, uninstallArgs))
            {
                // Wait for OneDrive to be uninstalled
            }

            // Hidden files do not count. Only the top level of the folder is enumerated
            var userFilesCount = Directory.Exists(userDataFolder)
                ? new DirectoryInfo(userDataFolder).EnumerateFileSystemInfos().Count(item => !item.Attributes.HasFlag(FileAttributes.Hidden))
                : 0;

            if (Directory.Exists(userDataFolder) && userFilesCount.Equals(0))
            {
                FolderRemover.Remove(userDataFolder);
            }

            App.Status.SetStep("Panel_Step_RestartingExplorer");

            try
            {
                // Do not restart File Explorer process automatically if it stops in order to unload libraries
                processService.SetAutoRestartShell(allow: false);
                processService.KillProcessByName(3000, "explorer");
                Thread.Sleep(3000);
            }
            finally
            {
                processService.SetAutoRestartShell(allow: true);
            }

            processService.KillProcessByName(3000, "UserOOBEBroker");

            // Make sure that the path is the OneDrive folder indeed ("Microsoft OneDrive" or "OneDrive")
            // If the uninstall string points to e.g. %SystemRoot%\SysWOW64\OneDriveSetup.exe, the path is %SystemRoot%, and it must not be removed
            if (Path.GetFileName(oneDriveFolder).EndsWith("OneDrive", StringComparison.OrdinalIgnoreCase))
            {
                UnregisterFileSyncShell(oneDriveFolder);
                FolderRemover.Remove(oneDriveFolder);
            }

            using var explorer = processService.StartProcessByName("explorer.exe");
            Thread.Sleep(3000);
            Registry.CurrentUser.DeleteSubKeyValue("Environment", "OneDrive");
            Registry.CurrentUser.DeleteSubKeyValue("Environment", "OneDriveConsumer");
            scheduledTaskService.UnregisterOneDriveTasks();
            App.Status.SetStep("Panel_Step_RemovingFiles");
            DeleteResources();

            if (userFilesCount > 0)
            {
                // The user's files are kept: open the folder after File Explorer was restarted to let the user decide what to do with them
                App.Logger.LogOneDriveUserFilesExist(userDataFolder, userFilesCount);
                using var userFolder = processService.StartProcessByName("explorer.exe", userDataFolder);
            }
        }

        /// <inheritdoc/>
        public bool UserIsLogged()
        {
            // Checking whether user is logged into OneDrive (Microsoft account)
            var personalEmail = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\OneDrive\\Accounts\\Personal", "UserEmail") as string ?? string.Empty;
            var businessEmail = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\OneDrive\\Accounts\\Business1", "UserEmail") as string ?? string.Empty;
            return !string.IsNullOrWhiteSpace(personalEmail) || !string.IsNullOrWhiteSpace(businessEmail);
        }

        private static string GetUserDataFolderOrDefault() => Registry.CurrentUser.GetSubKeyValue("Environment", "OneDrive") as string ?? string.Empty;

        private static string GetUninstallString()
        {
            // The same data that "Get-Package -ProviderName Programs" reads: the "Uninstall" registry keys of the current user and the machine.
            // Reading them directly does not start PowerShell and takes milliseconds instead of seconds
            var uninstallKeys = new (Microsoft.Win32.RegistryKey Hive, string Path)[]
            {
                (Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"),
                (Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"),
                (Registry.LocalMachine, "Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"),
            };

            foreach (var (hive, path) in uninstallKeys)
            {
                using var uninstallKey = hive.OpenSubKey(path);

                foreach (var appName in uninstallKey?.GetSubKeyNames() ?? [])
                {
                    using var appKey = uninstallKey!.OpenSubKey(appName);

                    if ("Microsoft OneDrive".Equals(appKey?.GetValue("DisplayName") as string, StringComparison.OrdinalIgnoreCase))
                    {
                        return appKey.GetValue("UninstallString") as string ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }

        private void DeleteResources()
        {
            var localAppData = GetFolderPath(SpecialFolder.LocalApplicationData);
            var startMenu = GetFolderPath(SpecialFolder.StartMenu);
            var programData = GetFolderPath(SpecialFolder.CommonApplicationData);
            var programFiles = GetFolderPath(SpecialFolder.ProgramFiles);
            var systemDrive = Path.GetPathRoot(SystemDirectory)!;

            var deletedResources = new List<string>()
            {
                Path.Combine(localAppData, "OneDrive"), Path.Combine(localAppData, "Microsoft\\OneDrive"), Path.Combine(localAppData, "Microsoft\\OneAuth"),
                Path.Combine(startMenu, "Programs\\OneDrive.lnk"), Path.Combine(programData, "Microsoft OneDrive"), Path.Combine(programFiles, "Microsoft OneDrive"),
                Path.Combine(systemDrive, "OneDriveTemp"),
            };

            deletedResources.ForEach(resource =>
            {
                if (Directory.Exists(resource))
                {
                    FolderRemover.Remove(resource);
                }
                else
                {
                    resource.TryDelete();
                }
            });

            Registry.CurrentUser.DeleteSubKeyTree("Software\\Microsoft\\OneDrive", false);
        }

        private void UnregisterFileSyncShell(string setupFolder)
        {
            var regSvr = $"{GetFolderPath(SpecialFolder.System)}\\regsvr32.exe";

            if (!Directory.Exists(setupFolder))
            {
                return;
            }

            // The library is in the version subfolders
            foreach (var dll in Directory.EnumerateDirectories(setupFolder).Select(folder => Path.Combine(folder, "FileSyncShell64.dll")).Where(File.Exists))
            {
                using var regSvrProcess = processService.WaitForExit(name: regSvr, arguments: $"/u /s \"{dll}\"");
            }
        }
    }
}
