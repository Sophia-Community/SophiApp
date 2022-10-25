<#
	.SYNOPSIS
	Set path to binary files for SophiApp.csproj

	.LINK
	https://github.com/Inestic
#>
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
			Write-Warning -Message "File: $CsProjPath not found!"
		}
	}
}

Set-BinaryLink -CsProjPath "SophiApp\SophiApp\SophiApp.csproj" -SearchPattern "Windows.winmd" -ChangePattern "      <HintPath>SophiApp\Binary\Windows.winmd</HintPath>"
Set-BinaryLink -CsProjPath "SophiApp\SophiApp\SophiApp.csproj" -SearchPattern "Microsoft.Deployment.WindowsInstaller.dll" -ChangePattern "      <HintPath>SophiApp\Binary\Microsoft.Deployment.WindowsInstaller.dll</HintPath>"
Set-BinaryLink -CsProjPath "SophiApp\SophiApp\SophiApp.csproj" -SearchPattern "System.Management.Automation.dll" -ChangePattern "      <HintPath>SophiApp\Binary\Microsoft.Deployment.WindowsInstaller.dll</HintPath>"
