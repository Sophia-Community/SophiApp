<#
	.SYNOPSIS
	Disable debug mode in Method.cs file

	.LINK
	https://github.com/Inestic
#>
$MethodsCS = "SophiApp\SophiApp\ViewModels\Methods.cs"
if (Test-Path -Path $MethodsCS)
{
	Write-Host "`nMethods.cs found"

	$MethodsContent = Get-Content -Path $MethodsCS
	$LineNumber = ($MethodsContent | Select-String -Pattern "DebugMode = true" | Select-Object -Last 1).LineNumber
	$FormatedString = $MethodsContent[$LineNumber - 1]
	$MethodsContent[$LineNumber - 1] = $FormatedString.Replace("true", "false")
	Set-Content -Path $MethodsCS -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nFile $MethodsCS saved"
}
else
{
	Write-Host "`nMethods.cs not found"
}
