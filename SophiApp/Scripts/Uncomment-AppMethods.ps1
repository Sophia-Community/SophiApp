<#
	.SYNOPSIS
	Comments in .cs files are removed

	.INPUTS
	None

	.OUTPUTS
	Comments in .cs files are removed

	.NOTES
	Designed for GitHub Actions
	
	.LINK
	https://github.com/Inestic

	.VERSION
	v1.0.0

	.DATE
	15.09.2021

	Copyright (c) 2021 Inestic
#>

Write-Host "Uncomment-AppMethods.ps1"

$ConditionsHelper = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Helpers\ConditionsHelper.cs"
$CommentPattern = "//"
$UpdatePattern = "//new NoNewVersion()"

Write-Host "`nPath to ConditionsHelper.cs: ""$ConditionsHelper"""

if (Test-Path -Path $ConditionsHelper)
{
	Write-Host "`nConditionsHelper.cs found"

	$MethodsContent = Get-Content -Path $ConditionsHelper
	$LineNumber = ($MethodsContent | Select-String -Pattern $UpdatePattern | Select-Object -Last 1).LineNumber
	$FormatedString = $MethodsContent[$LineNumber - 1]
	$MethodsContent[$LineNumber - 1] = $FormatedString.Replace($CommentPattern, $null)	
	Set-Content -Path $ConditionsHelper -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force
	Write-Host "`nFile ""$ConditionsHelper"" saved"
}
else
{
	Write-Host "`nConditionsHelper.cs not found"
}
