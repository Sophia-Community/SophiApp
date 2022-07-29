$Parameters = @{
	Uri             = "https://api.github.com/repos/Sophia-Community/SophiApp/releases"
	UseBasicParsing = $true
}
(Invoke-RestMethod @Parameters) | Select-Object -First 1 | ForEach-Object -Process {
	if ($_.prerelease)
	{
		$Script:IsReleaseString = "        private const bool IS_RELEASE = true;"
		Write-Host "`nIs Pre-Release: true"
	}
	else
	{
		$Script:IsReleasePattern = "        private const bool IS_RELEASE ="
	}
}

$ReleaseTag          = $args[0].Split("/") | Select-Object -Last 1
$AssemblyInfo        = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Properties\AssemblyInfo.cs"
$AssemblyPattern     = "AssemblyVersion"
$AssemblyFilePattern = "AssemblyFileVersion"
$AssemblyString      = '[assembly: AssemblyVersion("{0}")]' -f $ReleaseTag
$AssemblyFileString  = '[assembly: AssemblyFileVersion("{0}")]' -f $ReleaseTag

Write-Host "`nRelease tag: `"$ReleaseTag`""
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

$AppHelper = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\Helpers\AppHelper.cs"
Write-Host "`nPath to AppHelper.cs: ""$AppHelper"""

if (Test-Path -Path $AppHelper)
{
	Write-Host "`nAppHelper.cs found"

	$AppHelperContent = Get-Content -Path $AppHelper
	$IsReleaseLineNumber = ($AppHelperContent | Select-String -Pattern $Script:IsReleasePattern | Select-Object -Last 1).LineNumber
	$AppHelperContent[$IsReleaseLineNumber - 1] = $Script:IsReleaseString
	Set-Content -Path $AppHelper -Value $AppHelperContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nAppHelper.cs saved"
}
else
{
	Write-Host "`nAppHelper.cs not found"
}
