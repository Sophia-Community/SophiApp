1. Добавить возможность копировать описание функции, нажав в контекстном меню рамки "Копировать"

2. Функция для экспорта ico из файла. Понадобится в будущем

<https://gist.github.com/darkfall/1656050#gistcomment-1332369>

```powershell
#
Add-Type -AssemblyName System.Drawing
[System.Drawing.Icon]::ExtractAssociatedIcon("C:\WINDOWS\system32\control.exe").ToBitmap().Save("C:\Desktop\1\1.ico")

#
$Path = "$env:SystemRoot\system32\control.exe"
$FileName = "C:\Desktop\1\1.ico"
$Format = [System.Drawing.Imaging.ImageFormat]::Icon
Add-Type -AssemblyName System.Drawing
$Icon = [System.Drawing.Icon]::ExtractAssociatedIcon($Path) | Add-Member -MemberType NoteProperty -Name FullName -Value $Path -PassThru
$Icon.ToBitMap().Save($FileName,$Format)
```
3. <https://github.com/WindowsNotifications?q=&type=&language=c%23>

============================================================================================

## Кастомный scrollviewer

<http://codesdirectory.blogspot.com/2013/01/wpf-scrollviewer-control-style.html>

## Кастомный dots line progress bar

<https://www.jeff.wilcox.name/2010/08/performanceprogressbar/>

## LoadingRing

<https://github.com/zeluisping/LoadingIndicators.WPF/blob/master/src/LoadingIndicators.WPF/Styles/LoadingRing.xaml>

# Expands a Microsoft @-prefixed indirect string

https://github.com/SamuelArnold/StarKill3r/blob/master/Star%20Killer/Star%20Killer/bin/Debug/Scripts/SANS-SEC505-master/scripts/Day1-PowerShell/Expand-IndirectString.ps1

```powershell
$ExcludedAppxPackages = @(
	# Microsoft Desktop App Installer
	"Microsoft.DesktopAppInstaller",

	# Store Experience Host
	"Microsoft.StorePurchaseApp",

	# Microsoft Store
	"Microsoft.WindowsStore",

	# Web Media Extensions
	"Microsoft.WebMediaExtensions"
)
$AppxPackages = Get-AppxPackage -PackageTypeFilter Bundle -AllUsers | Where-Object -FilterScript {$_.Name -notin $ExcludedAppxPackages}
[Windows.Management.Deployment.PackageManager, Windows.Web, ContentType = WindowsRuntime]::new().FindPackages() | Select-Object -Property DisplayName, Logo -ExpandProperty Id | Where-Object -FilterScript {$_.Name -in $AppxPackages.Name} | Select-Object -Property Name, DisplayName, Logo | Format-Table -Wrap
```
