Clear-Host
$currentDir = $PSCommandPath | Split-Path -Parent | Split-Path -Parent
#$currentDir = (Split-Path -Path $PSCommandPath -Qualifier) + "\"
$scriptsDir = "{0}\SophiApp\SophiAppCE\bin\Debug" -f $currentDir
$settingsJson = "{0}\Settings\SettingsCE.json" -f $currentDir
$tags = "", "Privacy", "Ui", "ContextMenu", "StartMenu", "System", "TaskSheduler", "Security", "Game", "Uwp", "OneDrive"

if (Test-Path -Path $settingsJson)
{
	$json = Get-Content -Path $settingsJson -Encoding UTF8 | Out-String | ConvertFrom-Json
	$json | Foreach-Object {
		$jsonObj = $_		
		$scriptPath = "{0}\{1}" -f $scriptsDir, $jsonObj.Path

		if ($scriptPath)
		{
#			$sha = Get-FileHash -Path $scriptPath
#			$_ | Add-Member NoteProperty "Type" "SwitchBar" -Force
#			$_ | Add-Member NoteProperty "Sha256" (Get-FileHash -Path $scriptPath).Hash -Force
#			$jsonObj | Add-Member NoteProperty "Tag" $tags[$jsonObj.Id[0].ToString()] -Force
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