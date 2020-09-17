Clear-Host
$currentDir = $PSCommandPath | Split-Path -Parent | Split-Path -Parent
$scriptsDir = "{0}\SophiApp\SophiAppCE\bin\Debug" -f $currentDir
$settingsJson = "{0}\Settings\SettingsCE.json" -f $currentDir
$tags = @{"1"="Privacy";"2"="Ui";"3"="OneDrive";"4"="System";"5"="StartMenu";"6"="Uwp";"7"="Game";"8"="TaskSheduler";"9"="Security";"10"="ContextMenu"}

if (Test-Path -Path $settingsJson)
{
	$json = Get-Content -Path $settingsJson -Encoding UTF8 | Out-String | ConvertFrom-Json
	if ($json.Data.Count -gt 0)
	{
		$json.Data | Foreach-Object {
			$jsonObj = $_		
			$scriptPath = "{0}\{1}" -f $scriptsDir, $jsonObj.Path
			
			if (Test-Path -Path $scriptPath)
			{								
				$jsonObj | Add-Member NoteProperty "Sha256" (Get-FileHash -Path $scriptPath).Hash -Force
				$jsonObj | Add-Member NoteProperty "Tag" $tags[$jsonObj.Id[0].ToString()] -Force
			}
			
			else
			{
				Write-Host
				Write-Warning -Message "Файл ""$scriptPath"" не найден!"
			}
		}
		
		$json | ConvertTo-Json | Out-File -Encoding UTF8 -FilePath ($settingsJson.Replace(".json", ("_{0}.json"-f [DateTime]::Now.Ticks)))
		Write-Host "Конвертация завершена успешно!"
	}
	
	else
	{
		Write-Warning -Message "В файле ""Settings.json"" нет данных!"
	}	
}
else
{
	Write-Warning -Message "Файл ""Settings.json"" не найден!"
}