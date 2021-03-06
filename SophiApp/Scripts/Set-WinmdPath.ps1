<#
	.SYNOPSIS
	Set path to binary files for SophiApp.csproj

	.INPUTS
	None

	.OUTPUTS
	Set path to binary files for SophiApp.csproj

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

function Set-BinaryLink 
{
	[CmdletBinding()]
	param
	(
    	[Parameter(Mandatory=$True)]
    	[ValidateNotNull()]
    	[string]$CsProjPath,
		
    	[Parameter(Mandatory=$True)]
    	[ValidateNotNull()]
    	[string]$SearchPattern,
		
		[Parameter(Mandatory=$True)]
    	[ValidateNotNull()]
    	[string]$ChangePattern
	)
	
	process
	{
		if (Test-Path -Path $CsProjPath)
		{
			$Content = Get-Content -Path $CsProjPath -Encoding UTF8
			$LineNumber = ($Content | Select-String -Pattern $SearchPattern | Select-Object -Last 1).LineNumber
			$FormatedString = $Content[$LineNumber - 1]
			$Content[$LineNumber - 1] = $ChangePattern
			Set-Content -Path $CsProjPath -Value $Content -Confirm:$false -Encoding UTF8 -Force
		}
		else
		{
			Write-Warning -Message "File: ""$CsProjPath"" not found!"
		}
	}
}

$SophiAppCs = "{0}\{1}" -f (Split-Path -Path $PSScriptRoot -Parent), "SophiApp\SophiApp.csproj"

$WinMdSearchPattern = "Windows.winmd"
$WinmdChangePattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\Windows.winmd"

$WindowsInstallerPattern = "Microsoft.Deployment.WindowsInstaller.dll"
$WindowsInstallerChangePattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\Microsoft.Deployment.WindowsInstaller.dll"

$SystemManagementAutomationPattern = "System.Management.Automation.dll"
$SystemManagementAutomationChangePattern = "      <HintPath>{0}\{1}</HintPath>" -f (Split-Path -Path $PSScriptRoot -Parent), "Binary\System.Management.Automation.dll"


Set-BinaryLink -CsProjPath $SophiAppCs -SearchPattern $WinMdSearchPattern -ChangePattern $WinmdChangePattern
Set-BinaryLink -CsProjPath $SophiAppCs -SearchPattern $WindowsInstallerPattern -ChangePattern $WindowsInstallerChangePattern
Set-BinaryLink -CsProjPath $SophiAppCs -SearchPattern $SystemManagementAutomationPattern -ChangePattern $SystemManagementAutomationChangePattern
