<#
	.SYNOPSIS
	Download binary for SophiApp build action

	.INPUTS
	None

	.OUTPUTS
	Binary files in "Bin" folder

	.NOTES
	Designed for GitHub Actions
	
	.LINK
	https://github.com/farag2
	https://github.com/Inestic

	.VERSION
	v1.0.1

	.DATE
	24.01.2022

	Copyright (c) 2021 farag2, Inestic
	
	https://developer.microsoft.com/ru-ru/windows/downloads/sdk-archive/
	https://www.nuget.org/packages/Microsoft.Dism
	https://github.com/dahall/TaskScheduler
	https://github.com/JamesNK/Newtonsoft.Json
#>

$ReleaseBinDir = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\bin\Release\Bin"
$BinaryFiles = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\*"

if (-not (Test-Path -Path $ReleaseBinDir))
{
	New-Item -Path (Split-Path -Path $ReleaseBinDir -Parent) -Name Bin -ItemType Directory
}

# Download the microsoft.dism package
# https://github.com/jeffkl/ManagedDism
$Parameters = @{
	Uri             = "https://api.github.com/repos/jeffkl/ManagedDism/releases/latest"
	UseBasicParsing = $true
}
$ManagedDismLatestVersion = (Invoke-RestMethod @Parameters).tag_name.Replace("v", "")

$Parameters = @{
	Uri             = "https://globalcdn.nuget.org/packages/microsoft.dism.$ManagedDismLatestVersion.nupkg"
	OutFile         = "$ReleaseBinDir\microsoft.dism.nupkg"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Rename the acrhive to able to expand it
Rename-Item -Path "$ReleaseBinDir\microsoft.dism.nupkg" -NewName "$ReleaseBinDir\microsoft.dism.zip" -Force

# Extract Microsoft.Dism.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$ReleaseBinDir\microsoft.dism.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "lib/net40/Microsoft.Dism.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "$ReleaseBinDir\$($_.Name)", $true)}
$ZIP.Dispose()

# Download the TaskScheduler package
# https://www.nuget.org/packages/TaskScheduler
$Parameters = @{
	Uri             = "https://www.nuget.org/api/v2/package/TaskScheduler"
        OutFile         = "$ReleaseBinDir\TaskScheduler.zip"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Extract Microsoft.Win32.TaskScheduler.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$ReleaseBinDir\TaskScheduler.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "lib/net452/Microsoft.Win32.TaskScheduler.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "$ReleaseBinDir\$($_.Name)", $true)}
$ZIP.Dispose()

# Download the Newtonsoft.Json package
# https://github.com/JamesNK/Newtonsoft.Json
$Parameters = @{
	Uri             = "https://api.github.com/repos/JamesNK/Newtonsoft.Json/releases/latest"
	UseBasicParsing = $true
}
$NewtonsoftJsonLatestVersion = ((Invoke-RestMethod @Parameters).assets | Where-Object -FilterScript {$_.browser_download_url -match "Json"}).browser_download_url

$Parameters = @{
	Uri             = $NewtonsoftJsonLatestVersion
	OutFile         = "$ReleaseBinDir\Json.zip"
	UseBasicParsing = $true
}
Invoke-RestMethod @Parameters

# Extract Newtonsoft.Json.dll from the archive
Add-Type -Assembly System.IO.Compression.FileSystem
$ZIP = [IO.Compression.ZipFile]::OpenRead("$ReleaseBinDir\Json.zip")
$Entries = $ZIP.Entries | Where-Object -FilterScript {$_.FullName -eq "Bin/net45/Newtonsoft.Json.dll"}
$Entries | ForEach-Object -Process {[IO.Compression.ZipFileExtensions]::ExtractToFile($_, "$ReleaseBinDir\$($_.Name)", $true)}
$ZIP.Dispose()

Copy-Item -Path $BinaryFiles -Destination $ReleaseBinDir
Remove-Item -Path "$ReleaseBinDir\*.zip" -Force
