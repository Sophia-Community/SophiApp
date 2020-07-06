Clear-Host
$currentDir = $PSCommandPath | Split-Path -Parent | Split-Path -Parent
# $currentDir = (Split-Path -Path $PSCommandPath -Qualifier) + "\"
$scriptsDir = "{0}\SophiApp\SophiAppCE\bin\Debug" -f $currentDir
$settingsJson = "{0}\Settings\Settings.json" -f $currentDir

if (Test-Path -Path $settingsJson)
{
	$json = Get-Content -Path $settingsJson -Encoding UTF8 | Out-String | ConvertFrom-Json
	$json | Foreach-Object {
		$scriptPath = "{0}\{1}" -f $scriptsDir, $_.Path

		if ($scriptPath)
		{
			$sha = Get-FileHash -Path $scriptPath
			$_ | Add-Member NoteProperty "Type" "SwitchBar" -Force
			$_ | Add-Member NoteProperty "Sha256" (Get-FileHash -Path $scriptPath).Hash -Force
		}
		else
		{
			Write-Warning -Message "Файл $scriptPath не найден"
		}
	}

	ConvertTo-Json @($json) | Out-File -Encoding UTF8 -FilePath ($settingsJson.Replace(".json", ("_{0}.json"-f [DateTime]::Now.Ticks)))
}
else
{
	Write-Warning -Message "Файл ""Settings.json"" не найден"
}