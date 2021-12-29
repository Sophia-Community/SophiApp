using SophiApp.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace SophiApp.Helpers
{
    internal class UwpHelper
    {
        internal static IEnumerable<UwpElementDto> GetPackagesDto(bool forAllUsers = false)
        {
            var currentUserScript = @"# The following UWP apps will be excluded from the display
$ExcludedAppxPackages = @(
# Microsoft Desktop App Installer
'Microsoft.DesktopAppInstaller',

# Store Experience Host
'Microsoft.StorePurchaseApp',

# Microsoft Store
'Microsoft.WindowsStore',

# Windows Terminal
'Microsoft.WindowsTerminal',
'Microsoft.WindowsTerminalPreview',

# Web Media Extensions
'Microsoft.WebMediaExtensions'
)

$AppxPackages = Get-AppxPackage -PackageTypeFilter Bundle | Where-Object -FilterScript {$_.Name -notin $ExcludedAppxPackages}
$PackagesIds = [Windows.Management.Deployment.PackageManager, Windows.Web, ContentType = WindowsRuntime]::new().FindPackages() | Select-Object -Property DisplayName, Logo -ExpandProperty Id | Select-Object -Property Name, DisplayName, Logo

foreach ($AppxPackage in $AppxPackages)
{
	$PackageId = $PackagesIds | Where-Object -FilterScript {$_.Name -eq $AppxPackage.Name}

	if (-not $PackageId)
	{
		continue
	}

	 [PSCustomObject]@{
		Name            = $AppxPackage.Name
		PackageFullName = $AppxPackage.PackageFullName
		Logo            = $PackageId.Logo
		DisplayName     = $PackageId.DisplayName
	}
}";
            var allUsersScript = @"# The following UWP apps will be excluded from the display
$ExcludedAppxPackages = @(
# Microsoft Desktop App Installer
'Microsoft.DesktopAppInstaller',

# Store Experience Host
'Microsoft.StorePurchaseApp',

# Microsoft Store
'Microsoft.WindowsStore',

# Windows Terminal
'Microsoft.WindowsTerminal',
'Microsoft.WindowsTerminalPreview',

# Web Media Extensions
'Microsoft.WebMediaExtensions'
)

$AppxPackages = Get-AppxPackage -PackageTypeFilter Bundle -AllUsers | Where-Object -FilterScript {$_.Name -notin $ExcludedAppxPackages}
$PackagesIds = [Windows.Management.Deployment.PackageManager, Windows.Web, ContentType = WindowsRuntime]::new().FindPackages() | Select-Object -Property DisplayName, Logo -ExpandProperty Id | Select-Object -Property Name, DisplayName, Logo

foreach ($AppxPackage in $AppxPackages)
{
	$PackageId = $PackagesIds | Where-Object -FilterScript {$_.Name -eq $AppxPackage.Name}

	if (-not $PackageId)
	{
		continue
	}

	 [PSCustomObject]@{
		Name            = $AppxPackage.Name
		PackageFullName = $AppxPackage.PackageFullName
		Logo            = $PackageId.Logo
		DisplayName     = $PackageId.DisplayName
	}
}";

            return PowerShell.Create()
                             .AddScript(forAllUsers ? allUsersScript : currentUserScript)
                             .Invoke()
                             .Where(uwp => uwp.Properties["Logo"].Value != null)
                             .Select(uwp => new UwpElementDto()
                             {
                                 Name = uwp.Properties["Name"].Value as string,
                                 PackageFullName = uwp.Properties["PackageFullName"].Value as string,
                                 Logo = uwp.Properties["Logo"].Value.GetFirstValue<Uri>(),
                                 DisplayName = uwp.Properties["DisplayName"].Value.GetFirstValue<string>()
                             });
        }

        internal static void InstallPackage(string package)
        {
            var packageUri = new Uri(package);
            var packageManager = new PackageManager();
            var deploymentOperation = packageManager.AddPackageAsync(packageUri, null, DeploymentOptions.None);
            var opCompletedEvent = new ManualResetEvent(false);
            deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };
            opCompletedEvent.WaitOne();
        }

        internal static bool PackageExist(string packageName)
        {
            var sid = OsHelper.GetCurrentUserSid().Value;
            var packageManager = new PackageManager();
            return packageManager.FindPackagesForUser(sid)
                                 .Where(package => package.Id.Name == packageName)
                                 .Count() > 0;
        }

        internal static void RemovePackage(string packageName, bool allUsers)
        {
            var stopwatch = Stopwatch.StartNew();
            var packageManager = new PackageManager();
            var deploymentOperation = packageManager.RemovePackageAsync(packageName, allUsers ? RemovalOptions.RemoveForAllUsers : RemovalOptions.None);
            var opCompletedEvent = new ManualResetEvent(false);
            deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };
            opCompletedEvent.WaitOne();
            stopwatch.Stop();

            if (deploymentOperation.Status == AsyncStatus.Error)
            {
                var deploymentResult = deploymentOperation.GetResults();
                DebugHelper.UwpRemovedHasException(packageName, deploymentResult.ErrorText);
                return;
            }

            DebugHelper.UwpRemoved(packageName, stopwatch.Elapsed.TotalSeconds, deploymentOperation.Status);
        }
    }
}