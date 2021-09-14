<#
	.SYNOPSIS
	The AssemblyInfo.cs file set the release version that matches the release tag.

	.INPUTS
	GitHub release tag.

	.OUTPUTS
	Changes app version in AssemblyInfo.cs file.

	.NOTES
	Designed for GitHub Actions.
	
	.LINK
	https://github.com/Inestic

	.VERSION
	v1.0.0

	.DATE
	14.09.2021

	Copyright (c) 2021 Inestic
#>
Clear-Host
$ReleaseTag = $args[0]
$AssemblyInfo = "{0}\{1}"-f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Properties\AssemblyInfo.cs"
$AssemblyPattern = "AssemblyVersion"
$AssemblyFilePattern = "AssemblyFileVersion"
$AssemblyString = '[assembly: AssemblyVersion("{0}")]'-f $ReleaseTag
$AssemblyFileString = '[assembly: AssemblyFileVersion("{0}")]'-f $ReleaseTag

Write-Host "Release tag is: ""$ReleaseTag"""

if (Test-Path -Path $AssemblyInfo)
{
	Write-Host "File ""$AssemblyInfo"" found"
	$AssemblyContent = Get-Content -Path $AssemblyInfo
	$AssemblyLineNumber = ($AssemblyContent | Select-String -Pattern $AssemblyPattern | Select-Object -Last 1).LineNumber
	$AssemblyFileLineNumber = ($AssemblyContent | Select-String -Pattern $AssemblyFilePattern).LineNumber
	$AssemblyContent[$AssemblyLineNumber - 1] = $AssemblyString
	$AssemblyContent[$AssemblyFileLineNumber - 1] = $AssemblyFileString
	Set-Content -Path $AssemblyInfo -Value $AssemblyContent -Confirm:$false -Encoding UTF8 -Force
	Write-Host "File ""$AssemblyInfo"" saved"
}
else
{
	Write-Host "File ""$AssemblyInfo"" not found"
}