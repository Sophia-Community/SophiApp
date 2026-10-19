// <copyright file="OneDriveService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using System.Diagnostics;
    using static System.Environment;

    /// <inheritdoc/>
    public class OneDriveService : IOneDriveService
    {
        private readonly IHttpService httpService;
        private readonly IPowerShellService powerShellService;
        private readonly IProcessService processService;
        private readonly IScheduledTaskService scheduledTaskService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveService"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        /// <param name="processService">A service for working with Windows <see cref="Process"/> API.</param>
        /// <param name="scheduledTaskService">A service for working with Scheduled Task API.</param>
        public OneDriveService(
            IHttpService httpService,
            IPowerShellService powerShellService,
            IProcessService processService,
            IScheduledTaskService scheduledTaskService)
        {
            this.httpService = httpService;
            this.powerShellService = powerShellService;
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
        public string GetUserDataFolderOrDefault() => Registry.CurrentUser.OpenSubKey("Environment")?.GetValue("OneDrive") as string ?? string.Empty;

        /// <inheritdoc/>
        public void Install()
        {
            var setupFile = GetSetupFileOrDefault();

            if (!string.IsNullOrEmpty(setupFile))
            {
                processService.WaitForExit(setupFile);
            }
            else
            {
                var downloadPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders";
                // Get user Download folder path
                var downloadFolder = Registry.CurrentUser.OpenSubKey(downloadPath)?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") as string ?? string.Empty;
                var downloadFile = Path.Combine(downloadFolder, "OneDriveSetup.exe");
                httpService.DownloadOneDrive(downloadFile);
                processService.WaitForExit(downloadFile);
                Thread.Sleep(3000);
                File.Delete(downloadFile);
            }
        }

        /// <inheritdoc/>
        public bool IsInstalled()
        {
            var package = powerShellService.Invoke(@"Get-Package -Name ""Microsoft OneDrive"" -ProviderName Programs").FirstOrDefault();
            return package is not null;
        }

        /// <inheritdoc/>
        public void Uninstall()
        {
            var uninstallString = GetUninstallString();
            var uninstallFolder = uninstallString[..uninstallString.LastIndexOf('\\')];
            var uninstallFile = uninstallString[.. (uninstallString.IndexOf(".exe") + 4)];
            var uninstallArgs = uninstallString.Substring(uninstallString.IndexOf("/"));
            var userDataFolder = GetUserDataFolderOrDefault();
            // PowerShell is used to avoid going through all the files and folders, of which there may be many, to filter user and system files.
            var userFilesCount = Path.Exists(userDataFolder) ? powerShellService.Invoke<int>($"(Get-ChildItem -Path {userDataFolder} -ErrorAction Ignore | Measure-Object).Count") : 0;
            processService.KillProcessByName(3000, "OneDrive", "OneDriveSetup", "FileCoAuth");
            _ = processService.WaitForExit(uninstallFile, uninstallArgs);

            if (userFilesCount.Equals(0))
            {
                // Uses PowerShell to avoid the "Access Denied" error, not to go through all the files and folders, of which there may be many, to set the Normal attribute.
                // See https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it
                _ = powerShellService.Invoke("Remove-Item -Path $env:OneDrive -Recurse -Force -ErrorAction Ignore");
                processService.SetAutoRestartShell(allow: false);
                processService.KillProcessByName(3000, "explorer", "UserOOBEBroker");
                Thread.Sleep(3000);
                UnregisterFileSyncShell(uninstallFolder);
                processService.SetAutoRestartShell(allow: true);

                // Uses PowerShell to avoid the "Access Denied" error, not to go through all the files and folders, of which there may be many, to set the Normal attribute.
                // See https://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it
                _ = powerShellService.Invoke($"Remove-Item -Path \"{uninstallFolder}\" -Force -Recurse -ErrorAction Ignore");
                _ = processService.StartProcessByName("explorer.exe");
                Thread.Sleep(3000);
                Registry.CurrentUser.OpenSubKey("Environment", true)?.DeleteValue("OneDrive", false);
                Registry.CurrentUser.OpenSubKey("Environment", true)?.DeleteValue("OneDriveConsumer", false);
                scheduledTaskService.UnregisterOneDriveTasks();
                DeleteResources();
            }
            else
            {
                App.Logger.LogOneDriveUserFilesExist(userDataFolder, userFilesCount);
                processService.StartProcessByName("explorer.exe", userDataFolder);
            }
        }

        /// <inheritdoc/>
        public bool UserIsLogged()
        {
            // Checking whether user is logged into OneDrive (Microsoft account)
            var personalEmail = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\OneDrive\\Accounts\\Personal")
                ?.GetValue("UserEmail") as string ?? string.Empty;
            var businessEmail = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\OneDrive\\Accounts\\Business1")
                ?.GetValue("UserEmail") as string ?? string.Empty;
            return !(string.IsNullOrWhiteSpace(personalEmail) || string.IsNullOrWhiteSpace(businessEmail));
        }

        private string GetUninstallString()
        {
            var command = "Get-Package -Name \"Microsoft OneDrive\" -ErrorAction Ignore | ForEach-Object -Process {$_.Meta.Attributes[\"UninstallString\"]}";
            var invoke = powerShellService.Invoke(command)[0]?.BaseObject as string ?? string.Empty;
            return invoke[1..];
        }

        private void DeleteResources()
        {
            var localAppData = GetFolderPath(SpecialFolder.LocalApplicationData);
            var startMenu = GetFolderPath(SpecialFolder.StartMenu);
            var programData = GetFolderPath(SpecialFolder.CommonApplicationData);
            var programFiles = GetFolderPath(SpecialFolder.ProgramFiles);
            var systemDrive = Path.GetPathRoot(SystemDirectory) !;

            var deletedResources = new List<string>()
            {
                Path.Combine(localAppData, "OneDrive"), Path.Combine(localAppData, "Microsoft\\OneDrive"), Path.Combine(localAppData, "Microsoft\\OneAuth"),
                Path.Combine(startMenu, "Programs\\OneDrive.lnk"), Path.Combine(programData, "Microsoft OneDrive"), Path.Combine(programFiles, "Microsoft OneDrive"),
                Path.Combine(systemDrive, "OneDriveTemp"),
            };

            deletedResources.ForEach(r =>
            {
                if (Directory.Exists(r))
                {
                    Directory.Delete(r, true);
                }
                else if (File.Exists(r))
                {
                    File.Delete(r);
                }
            });

            Registry.CurrentUser.DeleteSubKeyTree("Software\\Microsoft\\OneDrive", false);
        }

        private void UnregisterFileSyncShell(string setupFolder)
        {
            var regSvr = $"{GetFolderPath(SpecialFolder.System)}\\regsvr32.exe";
            Directory.GetFileSystemEntries(path: setupFolder, searchPattern: "FileSyncShell64.dll", SearchOption.AllDirectories)
                .ForEach(dll => processService.WaitForExit(name: regSvr, arguments: $"/u /s \"{dll}\""));
        }
    }
}
