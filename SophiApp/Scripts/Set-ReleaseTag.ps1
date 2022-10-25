$Parameters = @{
	Uri             = "https://api.github.com/repos/Sophia-Community/SophiApp/releases"
	UseBasicParsing = $true
}
(Invoke-RestMethod @Parameters) | Select-Object -First 1 | ForEach-Object -Process {
	if ($_.prerelease)
	{
		$IsReleaseString = "        private const bool IS_RELEASE = false;"
		Write-Host "`nIs Pre-Release: true"
	}
	else
	{
		$IsReleaseString = "        private const bool IS_RELEASE = true;"
		Write-Host "`nIs Pre-Release: false"
	}
}

$ReleaseTag          = $args[0].Split("/") | Select-Object -Last 1
$AssemblyString      = '[assembly: AssemblyVersion("{0}")]' -f $ReleaseTag
$AssemblyFileString  = '[assembly: AssemblyFileVersion("{0}")]' -f $ReleaseTag

Write-Host "`nRelease tag: $ReleaseTag"

if (Test-Path -Path "SophiApp\SophiApp\Properties\AssemblyInfo.cs")
{
	Write-Host "`nAssemblyInfo.cs found"

	$AssemblyContent = Get-Content -Path "SophiApp\SophiApp\Properties\AssemblyInfo.cs"
	$AssemblyLineNumber = ($AssemblyContent | Select-String -Pattern "AssemblyVersion" | Select-Object -Last 1).LineNumber
	$AssemblyFileLineNumber = ($AssemblyContent | Select-String -Pattern "AssemblyFileVersion").LineNumber
	$AssemblyContent[$AssemblyLineNumber - 1] = $AssemblyString
	$AssemblyContent[$AssemblyFileLineNumber - 1] = $AssemblyFileString
	Set-Content -Path "SophiApp\SophiApp\Properties\AssemblyInfo.cs" -Value $AssemblyContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nFile SophiApp\SophiApp\Properties\AssemblyInfo.cs saved"
}
else
{
	Write-Host "`nAssemblyInfo.cs not found"
}

if (Test-Path -Path "SophiApp\SophiApp\Helpers\AppHelper.cs")
{
	Write-Host "`nAppHelper.cs found"

	$AppHelperContent = Get-Content -Path "SophiApp\SophiApp\Helpers\AppHelper.cs"
	$IsReleaseLineNumber = ($AppHelperContent | Select-String -Pattern "        private const bool IS_RELEASE =" | Select-Object -Last 1).LineNumber
	$AppHelperContent[$IsReleaseLineNumber - 1] = $Script:IsReleaseString
	Set-Content -Path "SophiApp\SophiApp\Helpers\AppHelper.cs" -Value $AppHelperContent -Confirm:$false -Encoding UTF8 -Force

	Write-Host "`nAppHelper.cs saved"
}
else
{
	Write-Host "`nAppHelper.cs not found"
}
