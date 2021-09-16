<#
	.SYNOPSIS
	Disable debug mode in Method.cs file.

	.INPUTS
	None.

	.OUTPUTS
	Changes debug mode property in ViewModel Methods.cs file.

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

$MethodsCS = "{0}\{1}"-f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\ViewModels\Methods.cs"
$DebugModePattern = "DebugMode = true"
Write-Host "Path to Methods.cs: ""$MethodsCS"""

if (Test-Path -Path $MethodsCS)
{
	Write-Host "Methods.cs found"
	$MethodsContent = Get-Content -Path $MethodsCS
	$LineNumber = ($MethodsContent | Select-String -Pattern $DebugModePattern | Select-Object -Last 1).LineNumber
	$FormatedString = $MethodsContent[$LineNumber - 1]
	$MethodsContent[$LineNumber - 1] = $FormatedString.Replace("true", "false")	
	Set-Content -Path $MethodsCS -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force
	Write-Host "File ""$MethodsCS"" saved"
}
else
{
	Write-Host "Methods.cs not found"
}