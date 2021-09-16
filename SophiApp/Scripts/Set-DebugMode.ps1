<#
	.SYNOPSIS
	Sets the debug mode depending on the release version.

	.INPUTS
	GitHub release tag.

	.OUTPUTS
	Changes app version in ViewModel Methods.cs file.

	.NOTES
	Designed for GitHub Actions.
	
	.LINK
	https://github.com/Inestic

	.VERSION
	v1.0.0

	.DATE
	16.09.2021

	Copyright (c) 2021 Inestic
#>

Write-Host "Is prerelease: $args[0]"
#$ReleaseTag = $args[0].Split("/") | Select-Object -Last 1
#$AssemblyInfo = "{0}\{1}"-f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Properties\AssemblyInfo.cs"
#$AssemblyPattern = "AssemblyVersion"
#$AssemblyFilePattern = "AssemblyFileVersion"
#$AssemblyString = '[assembly: AssemblyVersion("{0}")]'-f $ReleaseTag
#$AssemblyFileString = '[assembly: AssemblyFileVersion("{0}")]'-f $ReleaseTag
#
#Write-Host "Release tag: ""$ReleaseTag"""
#Write-Host "Path to AssemblyInfo.cs: ""$AssemblyInfo"""
#
#if (Test-Path -Path $AssemblyInfo)
#{
#	Write-Host "AssemblyInfo.cs found"
#	$AssemblyContent = Get-Content -Path $AssemblyInfo
#	$AssemblyLineNumber = ($AssemblyContent | Select-String -Pattern $AssemblyPattern | Select-Object -Last 1).LineNumber
#	$AssemblyFileLineNumber = ($AssemblyContent | Select-String -Pattern $AssemblyFilePattern).LineNumber
#	$AssemblyContent[$AssemblyLineNumber - 1] = $AssemblyString
#	$AssemblyContent[$AssemblyFileLineNumber - 1] = $AssemblyFileString
#	Set-Content -Path $AssemblyInfo -Value $AssemblyContent -Confirm:$false -Encoding UTF8 -Force
#	Write-Host "File ""$AssemblyInfo"" saved"
#}
#else
#{
#	Write-Host "AssemblyInfo.cs not found"
#}