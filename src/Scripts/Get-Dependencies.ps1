<#
	.SYNOPSIS
	Download binaries for SophiApp build action

	.NOTES
	https://developer.microsoft.com/ru-ru/windows/downloads/sdk-archive/
	https://www.nuget.org/packages/Microsoft.Dism
	https://github.com/dahall/TaskScheduler
	https://github.com/JamesNK/Newtonsoft.Json
#>
New-Item -Path src\Binary -ItemType Directory -Force

# https://www.nuget.org/packages/Microsoft.Dism
Write-Verbose -Message Microsoft.Dism -Verbose
$Parameters = @{
	Uri             = "https://www.nuget.org/api/v2/package/Microsoft.Dism"
	OutFile         = "$PSScriptRoot\microsoft.dism.zip"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Extract Microsoft.Dism.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$PSScriptRoot\microsoft.dism.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "lib/net40/Microsoft.Dism.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "src\Binary\$($_.Name)", $true)}
$ZIP.Dispose()

# https://www.nuget.org/packages/WiX
Write-Verbose -Message Microsoft.Deployment.WindowsInstaller -Verbose
$Parameters = @{
	Uri             = "https://www.nuget.org/api/v2/package/WiX"
	OutFile         = "$PSScriptRoot\wix.zip"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Extract Microsoft.Deployment.WindowsInstaller.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$PSScriptRoot\wix.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "tools/Microsoft.Deployment.WindowsInstaller.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "src\Binary\$($_.Name)", $true)}
$ZIP.Dispose()

# https://www.nuget.org/packages/TaskScheduler
Write-Verbose -Message TaskScheduler -Verbose
$Parameters = @{
	Uri             = "https://www.nuget.org/api/v2/package/TaskScheduler"
	OutFile         = "$PSScriptRoot\TaskScheduler.zip"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Extract Microsoft.Win32.TaskScheduler.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$PSScriptRoot\TaskScheduler.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "lib/net452/Microsoft.Win32.TaskScheduler.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "src\Binary\$($_.Name)", $true)}
$ZIP.Dispose()

# https://www.nuget.org/packages/Newtonsoft.Json
Write-Verbose -Message Newtonsoft.Json -Verbose
$Parameters = @{
	Uri             = "https://www.nuget.org/api/v2/package/Newtonsoft.Json"
	OutFile         = "$PSScriptRoot\newtonsoft.json.zip"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Extract Newtonsoft.Json.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$PSScriptRoot\newtonsoft.json.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "lib/net45/Newtonsoft.Json.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "src\Binary\$($_.Name)", $true)}
$ZIP.Dispose()

# Coping Windows.winmd
Write-Verbose -Message Windows.winmd -Verbose
$Parameters = @{
	Path        = "${env:ProgramFiles(x86)}\Windows Kits\10\UnionMetadata\10.0.19041.0\Windows.winmd"
	Destination = "src\Binary"
}
Copy-Item @Parameters

# Coping System.Management.Automation.dll
Write-Verbose -Message System.Management.Automation.dll -Verbose
$Parameters = @{
	Path        = "${env:ProgramFiles(x86)}\Reference Assemblies\Microsoft\WindowsPowerShell\3.0\System.Management.Automation.dll"
	Destination = "src\Binary"
}
Copy-Item @Parameters
