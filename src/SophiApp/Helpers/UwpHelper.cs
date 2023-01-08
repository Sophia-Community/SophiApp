using SophiApp.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace SophiApp.Helpers
{
    internal class UwpHelper
    {
        internal static Package GetPackage(string packageName)
        {
            var sid = OsHelper.GetCurrentUserSid().Value;
            var packageManager = new PackageManager();
            return packageManager.FindPackagesForUser(sid).First(package => package.Id.Name.Equals(packageName));
        }

        internal static IEnumerable<UwpElementDto> GetPackagesDto(bool forAllUsers = false)
        {
            var currentUserScript = @"
# The following UWP apps will be excluded from the display
$ExcludedAppxPackages = @(
# Microsoft Desktop App Installer
'Microsoft.DesktopAppInstaller',

# Store Experience Host
'Microsoft.StorePurchaseApp',

# Notepad
'Microsoft.WindowsNotepad',

# Microsoft Store
'Microsoft.WindowsStore',

# Windows Terminal
'Microsoft.WindowsTerminal',
'Microsoft.WindowsTerminalPreview',

# Web Media Extensions
'Microsoft.WebMediaExtensions',

# AV1 Video Extension
'Microsoft.AV1VideoExtension',

# HEVC Video Extensions from Device Manufacturer
'Microsoft.HEVCVideoExtension',

# Raw Image Extension
'Microsoft.RawImageExtension',

# HEIF Image Extensions
'Microsoft.HEIFImageExtension',

# MPEG-2 Video Extension
'Microsoft.MPEG2VideoExtension'
)

$AppxPackages = Get-AppxPackage -PackageTypeFilter Bundle | Where-Object -FilterScript {$_.Name -notin $ExcludedAppxPackages}

# The Bundle packages contains no Microsoft Teams
if (Get-AppxPackage -Name MicrosoftTeams -AllUsers:$false)
{
	# Temporarily hack: due to the fact that there are actually two Microsoft Teams packages, we need to choose the first one to display
	$AppxPackages += Get-AppxPackage -Name MicrosoftTeams -AllUsers:$false | Select-Object -Index 0
}

# The Bundle packages contains no Spotify
if (Get-AppxPackage -Name SpotifyAB.SpotifyMusic -AllUsers:$false)
{
	# Temporarily hack: due to the fact that there are actually two Microsoft Teams packages, we need to choose the first one to display
	$AppxPackages += Get-AppxPackage -Name SpotifyAB.SpotifyMusic -AllUsers:$false | Select-Object -Index 0
}

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
            var allUsersScript = @"
# The following UWP apps will be excluded from the display
$ExcludedAppxPackages = @(
# Microsoft Desktop App Installer
'Microsoft.DesktopAppInstaller',

# Store Experience Host
'Microsoft.StorePurchaseApp',

# Notepad
'Microsoft.WindowsNotepad',

# Microsoft Store
'Microsoft.WindowsStore',

# Windows Terminal
'Microsoft.WindowsTerminal',
'Microsoft.WindowsTerminalPreview',

# Web Media Extensions
'Microsoft.WebMediaExtensions',

# AV1 Video Extension
'Microsoft.AV1VideoExtension',

# HEVC Video Extensions from Device Manufacturer
'Microsoft.HEVCVideoExtension',

# Raw Image Extension
'Microsoft.RawImageExtension',

# HEIF Image Extensions
'Microsoft.HEIFImageExtension'
)

$AppxPackages = Get-AppxPackage -PackageTypeFilter Bundle -AllUsers | Where-Object -FilterScript {$_.Name -notin $ExcludedAppxPackages}

# The Bundle packages contains no Microsoft Teams
if (Get-AppxPackage -Name MicrosoftTeams -AllUsers:$true)
{
	# Temporarily hack: due to the fact that there are actually two Microsoft Teams packages, we need to choose the first one to display
	$AppxPackages += Get-AppxPackage -Name MicrosoftTeams -AllUsers:$true | Select-Object -Index 0
}

# The Bundle packages contains no Spotify
if (Get-AppxPackage -Name SpotifyAB.SpotifyMusic -AllUsers:$true)
{
	# Temporarily hack: due to the fact that there are actually two Microsoft Teams packages, we need to choose the first one to display
	$AppxPackages += Get-AppxPackage -Name SpotifyAB.SpotifyMusic -AllUsers:$true | Select-Object -Index 0
}

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

            return PowerShell.Create().AddScript(forAllUsers ? allUsersScript : currentUserScript).Invoke()
                             .Where(uwp => uwp.Properties["Logo"].Value != null)
                             .Select(uwp => new UwpElementDto()
                             {
                                 Name = uwp.Properties["Name"].Value as string,
                                 PackageFullName = uwp.Properties["PackageFullName"].Value as string,
                                 Logo = uwp.Properties["Logo"].Value.GetFirstValue<Uri>(),
                                 DisplayName = uwp.Properties["DisplayName"].Value.GetFirstValue<string>()
                             });
        }

        internal static Version GetVersion(string package)
        {
            var packageVersion = GetPackage(package).Id.Version;
            return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
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
            return packageManager.FindPackagesForUser(sid).Where(package => package.Id.Name == packageName).Count() > 0;
        }

        internal static void RemovePackage(string packageFullName, bool allUsers)
        {
            var stopwatch = Stopwatch.StartNew();
            var packageManager = new PackageManager();
            var deploymentOperation = packageManager.RemovePackageAsync(packageFullName, allUsers ? RemovalOptions.RemoveForAllUsers : RemovalOptions.None);
            var opCompletedEvent = new ManualResetEvent(false);
            deploymentOperation.Completed = (depProgress, status) => { opCompletedEvent.Set(); };
            opCompletedEvent.WaitOne();
            stopwatch.Stop();

            if (deploymentOperation.Status == AsyncStatus.Error)
            {
                var deploymentResult = deploymentOperation.GetResults();
                DebugHelper.UwpRemovedHasException(packageFullName, deploymentResult.ErrorText);
                return;
            }

            DebugHelper.UwpRemoved(packageFullName, stopwatch.Elapsed.TotalSeconds, deploymentOperation.Status);
        }
    }
}