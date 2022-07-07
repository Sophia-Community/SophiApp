<#
	.SYNOPSIS
	Download the latest SophiApp version

	.EXAMPLE Download the latest SophiApp version
	irm app.sophi.app | iex
#>
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

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
	Uri              = "https://raw.githubusercontent.com/Sophia-Community/SophiApp/master/sophiapp_versions.json"
	UseBasicParsing  = $true
}
$LatestRelease = (Invoke-RestMethod @Parameters).SophiApp_release
$Parameters = @{
	Uri             = "https://github.com/Sophia-Community/SophiApp/releases/download/$LatestRelease/SophiApp.zip"
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

$SetForegroundWindow = @{
	Namespace = "WinAPI"
	Name      = "ForegroundWindow"
	Language  = "CSharp"
	MemberDefinition = @"
[DllImport("user32.dll")]
public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
[DllImport("user32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
public static extern bool SetForegroundWindow(IntPtr hWnd);
"@
}
if (-not ("WinAPI.ForegroundWindow" -as [type]))
{
	Add-Type @SetForegroundWindow
}

Start-Sleep -Seconds 1

Get-Process -Name explorer | Where-Object -FilterScript {$_.MainWindowTitle -eq "SophiApp"} | ForEach-Object -Process {
	# Show window, if minimized
	[WinAPI.ForegroundWindow]::ShowWindowAsync($_.MainWindowHandle, 5)

	# Force move the console window to the foreground
	[WinAPI.ForegroundWindow]::SetForegroundWindow($_.MainWindowHandle)
} | Out-Null

Write-Information -MessageData "" -InformationAction Continue
Write-Verbose -Message "Archive was expanded to `"$DownloadsFolder\SophiApp`"" -Verbose
