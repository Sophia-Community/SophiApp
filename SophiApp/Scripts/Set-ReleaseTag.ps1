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

$ReleaseTag = $args[0].Split("/") | Select-Object -Last 1
$AssemblyInfo = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Properties\AssemblyInfo.cs"
$AssemblyPattern = "AssemblyVersion"
$AssemblyFilePattern = "AssemblyFileVersion"
$AssemblyString = '[assembly: AssemblyVersion("{0}")]' -f $ReleaseTag
$AssemblyFileString = '[assembly: AssemblyFileVersion("{0}")]' -f $ReleaseTag

Write-Host "`nRelease tag: ""$ReleaseTag"""
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

Write-Host "test"
