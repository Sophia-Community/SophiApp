# Set path to binary files for SophiApp.csproj
function Set-BinaryLink
{
	[CmdletBinding()]
	param
	(
		[Parameter(Mandatory = $true)]
		[ValidateNotNull()]
		[string]
		$CsProjPath,

		[Parameter(Mandatory = $true)]
		[ValidateNotNull()]
		[string]
		$SearchPattern,

		[Parameter(Mandatory = $true)]
		[ValidateNotNull()]
		[string]
		$ChangePattern
	)

	process
	{
		$Content = Get-Content -Path $CsProjPath -Encoding UTF8
		$LineNumber = ($Content | Select-String -Pattern $SearchPattern | Select-Object -Last 1).LineNumber
		$FormatedString = $Content[$LineNumber - 1]
		$Content[$LineNumber - 1] = $ChangePattern

		Set-Content -Path $CsProjPath -Value $Content -Confirm:$false -Encoding UTF8 -Force
	}
}

$WinMdSearchPattern = "Windows.winmd"
$WinmdChangePattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\Windows.winmd"

$WindowsInstallerPattern = "Microsoft.Deployment.WindowsInstaller.dll"
$WindowsInstallerChangePattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\Microsoft.Deployment.WindowsInstaller.dll"

$SystemManagementAutomationPattern = "System.Management.Automation.dll"
$SystemManagementAutomationChangePattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\System.Management.Automation.dll"


Set-BinaryLink -CsProjPath $SophiAppCs -SearchPattern $WinMdSearchPattern -ChangePattern $WinmdChangePattern
Set-BinaryLink -CsProjPath $SophiAppCs -SearchPattern $WindowsInstallerPattern -ChangePattern $WindowsInstallerChangePattern
Set-BinaryLink -CsProjPath $SophiAppCs -SearchPattern $SystemManagementAutomationPattern -ChangePattern $SystemManagementAutomationChangePattern
