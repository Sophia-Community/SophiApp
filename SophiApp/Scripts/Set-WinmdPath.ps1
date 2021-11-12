<#
	.SYNOPSIS
	Set path to Windows.md for SophiApp.csproj

	.INPUTS
	None

	.OUTPUTS
	Set path to Windows.md for SophiApp.csproj

	.NOTES
	Designed for GitHub Actions
	
	.LINK
	https://github.com/Inestic

	.VERSION
	v1.0.0

	.DATE
	02.11.2021

	Copyright (c) 2021 Inestic
#>
$SophiAppCs = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\SophiApp.csproj"
$SearchPattern = "Windows.winmd"
$WinmdPattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\Windows.winmd"

if (Test-Path -Path $SophiAppCs)
{
	Write-Host "`nSophiApp.csproj was found"

	$CsproContent = Get-Content -Path $SophiAppCs
	$LineNumber = ($CsproContent | Select-String -Pattern $SearchPattern | Select-Object -Last 1).LineNumber
	$FormatedString = $CsproContent[$LineNumber - 1]
	$CsproContent[$LineNumber - 1] = $WinmdPattern
	Set-Content -Path $SophiAppCs -Value $CsproContent -Confirm:$false -Encoding UTF8 -Force
	Write-Host "`nFile ""SophiApp.csproj"" changed and saved"
}
else
{
	Write-Host "`nSophiApp.csproj not found"
}
