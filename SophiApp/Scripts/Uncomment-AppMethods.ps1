<#
	.SYNOPSIS
	Remove comments in .cs files

	.LINK
	https://github.com/Inestic

	Copyright (c) 2021 Inestic
#>
if (Test-Path -Path "SophiApp\SophiApp\Helpers\StartupConditionsHelper.cs")
{
	Write-Host "`nConditionsHelper.cs found"

	$MethodsContent = Get-Content -Path "SophiApp\SophiApp\Helpers\StartupConditionsHelper.cs"
	$LineNumber = ($MethodsContent | Select-String -Pattern "//new NewVersionCondition()" | Select-Object -Last 1).LineNumber
	$FormatedString = $MethodsContent[$LineNumber - 1]
	$MethodsContent[$LineNumber - 1] = $FormatedString.Replace("//", $null)
	Set-Content -Path "SophiApp\SophiApp\Helpers\StartupConditionsHelper.cs" -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nFile SophiApp\SophiApp\Helpers\StartupConditionsHelper.cs saved"
}
else
{
	Write-Host "`nConditionsHelper.cs not found"
}
