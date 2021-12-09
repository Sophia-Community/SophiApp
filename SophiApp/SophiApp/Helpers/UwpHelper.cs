using SophiApp.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
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
		Logo            = $PackageId.Logo -replace 'file:///', ''
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
		Logo            = $PackageId.Logo -replace 'file:///', ''
		DisplayName     = $PackageId.DisplayName
	}
}";

            foreach (var uwp in PowerShell.Create().AddScript(forAllUsers ? allUsersScript : currentUserScript).Invoke())
            {
                yield return new UwpElementDto()
                {
                    Name = uwp.Properties["Name"].Value as string,
                    PackageFullName = uwp.Properties["PackageFullName"].Value as string,
                    Logo = uwp.Properties["Logo"].Value as string,
                    DisplayName = uwp.Properties["DisplayName"].Value as string
                };
            }
        }

        internal static bool PackageExist(string packageName) => new PackageManager().FindPackages()
                                                                                     .Select(package => package.Id.Name)
                                                                                     .Contains(packageName);
    }
}