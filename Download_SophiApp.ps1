<#
	.SYNOPSIS
	Download the latest SophiApp version

	.EXAMPLE
	iwr app.sophia.team -useb | iex

	.NOTES
	Current user
#>

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

if ($Host.Version.Major -eq 5)
{
	# Progress bar can significantly impact cmdlet performance
	# https://github.com/PowerShell/PowerShell/issues/2138
	$Script:ProgressPreference = "SilentlyContinue"
}

if ((Get-Location).Path -eq $env:USERPROFILE)
{
	$DownloadsFolder = Get-ItemPropertyValue -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" -Name "{374DE290-123F-4565-9164-39C4925E467B}"
}
elseif ((Get-Location).Path -eq "$env:SystemRoot\System32")
{
	$DownloadsFolder = Get-ItemPropertyValue -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" -Name "{374DE290-123F-4565-9164-39C4925E467B}"
}
else
{
	$DownloadsFolder = (Get-Location).Path
}

$Parameters = @{
	Uri             = "https://api.github.com/repos/Sophia-Community/SophiApp/releases/latest"
	UseBasicParsing = $true
}
$LatestRelease = (Invoke-RestMethod @Parameters).assets.browser_download_url
$Parameters = @{
	Uri             = $LatestRelease
	OutFile         = "$DownloadsFolder\SophiApp.zip"
	UseBasicParsing = $true
	Verbose         = $true
}
Invoke-WebRequest @Parameters

$Parameters = @{
	Path            = "$DownloadsFolder\SophiApp.zip"
	DestinationPath = "$DownloadsFolder"
	Force           = $true
}
Expand-Archive @Parameters

Remove-Item -Path "$DownloadsFolder\SophiApp.zip" -Force
Start-Sleep -Second 1
Invoke-Item -Path "$DownloadsFolder\SophiApp"

# https://github.com/PowerShell/PowerShell/issues/21070
$CompilerParameters = [System.CodeDom.Compiler.CompilerParameters]::new("System.dll")
$CompilerParameters.TempFiles = [System.CodeDom.Compiler.TempFileCollection]::new($env:TEMP, $false)
$CompilerParameters.GenerateInMemory = $true
$Signature = @{
	Namespace          = "WinAPI"
	Name               = "ForegroundWindow"
	Language           = "CSharp"
	CompilerParameters = $CompilerParameters
	MemberDefinition   = @"
[DllImport("user32.dll")]
public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
[DllImport("user32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
public static extern bool SetForegroundWindow(IntPtr hWnd);
"@
	}

# PowerShell 7 has CompilerOptions argument instead of CompilerParameters as PowerShell 5 has
# https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/add-type#-compileroptions
if ($Host.Version.Major -eq 7)
{
	$Signature.Remove("CompilerParameters")
	$Signature.Add("CompilerOptions", $CompilerParameters)
}

if (-not ("WinAPI.ForegroundWindow" -as [type]))
{
	Add-Type @Signature
}

Start-Sleep -Seconds 1

Get-Process -Name explorer | Where-Object -FilterScript {$_.MainWindowTitle -eq "SophiApp"} | ForEach-Object -Process {
	# Show window, if minimized
	[WinAPI.ForegroundWindow]::ShowWindowAsync($_.MainWindowHandle, 5)

	# Force move the console window to the foreground
	[WinAPI.ForegroundWindow]::SetForegroundWindow($_.MainWindowHandle)
} | Out-Null
