<#
	.SYNOPSIS
	Comments for testing in .cs files are removed

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
Clear-Host

$MethodsCS = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\ViewModels\Methods.cs"
$CommentPattern = "//"
$UpdatePattern = "//await UpdateIsAvailableAsync()"

Write-Host "`nPath to Methods.cs: ""$MethodsCS"""

if (Test-Path -Path $MethodsCS)
{
	Write-Host "`nMethods.cs found"

	$MethodsContent = Get-Content -Path $MethodsCS
	$LineNumber = ($MethodsContent | Select-String -Pattern $UpdatePattern | Select-Object -Last 1).LineNumber
	$FormatedString = $MethodsContent[$LineNumber - 1]
	$MethodsContent[$LineNumber - 1] = $FormatedString.Replace($CommentPattern, $null)	
	Set-Content -Path $MethodsCS -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nFile ""$MethodsCS"" saved"
}
else
{
	Write-Host "`nMethods.cs not found"
}
