<#
	.SYNOPSIS
	The AssemblyInfo.cs file set the release version that matches the release tag

	.INPUTS
	GitHub release tag

	.OUTPUTS
	Changes app version in AssemblyInfo.cs file

	.NOTES
	Designed for GitHub Actions
	
	.LINK
	https://github.com/Inestic

	.VERSION
	v1.0.0

	.DATE
	14.09.2021

	Copyright (c) 2021 Inestic
#>
$ReleaseApi = "https://api.github.com/repos/Sophia-Community/SophiApp/releases"
$ReleaseResponse = (Invoke-WebRequest -Uri $ReleaseApi | ConvertFrom-Json) | Select-Object -First 1
[string]$IsRelease = !$ReleaseResponse.prerelease
$ReleaseTag = $args[0].Split("/") | Select-Object -Last 1
$AppHelper = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Helpers\AppHelper.cs"
$IsReleasePattern = "        private const bool IS_RELEASE ="
$IsReleaseString = "        private const bool IS_RELEASE = {0};" -f $IsRelease.ToLower()

$AssemblyInfo = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Properties\AssemblyInfo.cs"
$AssemblyPattern = "AssemblyVersion"
$AssemblyFilePattern = "AssemblyFileVersion"
$AssemblyString = '[assembly: AssemblyVersion("{0}")]' -f $ReleaseTag
$AssemblyFileString = '[assembly: AssemblyFileVersion("{0}")]' -f $ReleaseTag

Write-Host "`nRelease tag: ""$ReleaseTag"""
Write-Host "`nIs Pre-Release: $($ReleaseResponse.prerelease)"
Write-Host "`nPath to AssemblyInfo.cs: ""$AssemblyInfo"""

if (Test-Path -Path $AssemblyInfo)
{
	Write-Host "`nAssemblyInfo.cs found"

	$AssemblyContent = Get-Content -Path $AssemblyInfo
	$AssemblyLineNumber = ($AssemblyContent | Select-String -Pattern $AssemblyPattern | Select-Object -Last 1).LineNumber
	$AssemblyFileLineNumber = ($AssemblyContent | Select-String -Pattern $AssemblyFilePattern).LineNumber
	$AssemblyContent[$AssemblyLineNumber - 1] = $AssemblyString
	$AssemblyContent[$AssemblyFileLineNumber - 1] = $AssemblyFileString
	Set-Content -Path $AssemblyInfo -Value $AssemblyContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nFile ""$AssemblyInfo"" saved"
}
else
{
	Write-Host "`nAssemblyInfo.cs not found"
}

Write-Host "`nPath to AppHelper.cs: ""$AppHelper"""

if (Test-Path -Path $AppHelper)
{
	Write-Host "`nAppHelper.cs found"
	
	$AppHelperContent = Get-Content -Path $AppHelper
	$IsReleaseLineNumber = ($AppHelperContent | Select-String -Pattern $IsReleasePattern | Select-Object -Last 1).LineNumber
	$AppHelperContent[$IsReleaseLineNumber - 1] = $IsReleaseString
	Set-Content -Path $AppHelper -Value $AppHelperContent -Confirm:$false -Encoding UTF8 -Force	
}
else
{
	Write-Host "`nAppHelper.cs not found"
}
