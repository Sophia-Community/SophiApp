Clear-Host
$CurrentDir = Split-Path -Path (Get-Location).Path
$ScriptsDir = "{0}\SophiApp\SophiAppCE\bin\Debug" -f $CurrentDir
$SettingsJSON = "{0}\Settings\SettingsCE.json" -f $CurrentDir
$Tags =@{
	"1" = "Privacy"
	"2" = "Ui"
	"3" = "OneDrive"
	"4" = "System"
	"5" = "StartMenu"
	"6" = "Uwp"
	"7" = "Game"
	"8" = "TaskSheduler"
	"9" = "Security"
	"10" = "ContextMenu"
}

if (Test-Path -Path $SettingsJSON)
{
	$JSON = Get-Content -Path $SettingsJSON -Encoding UTF8 | Out-String | ConvertFrom-Json
	if ($JSON.Data.Count -gt 0)
	{
		$JSON.Data | Foreach-Object {			
			$JSONObj = $_
			$ScriptPath = "{0}\{1}" -f $ScriptsDir, $JSONObj.Path
			Write-Progress -Activity "Конвертация данных json" -Status "Конвертируется Id: ($JSONObj.Id)"

			if (Test-Path -Path $ScriptPath)
			{
				$JSONObj | Add-Member NoteProperty -Name "Sha256" -Value (Get-FileHash -Path $ScriptPath).Hash -Force
				$JSONObj | Add-Member NoteProperty -Name "Tag" -Value $Tags[$JSONObj.Id[0].ToString()] -Force
			}
			else
			{
				Write-Warning -Message "Файл `"$ScriptPath`" не найден!"
			}
		}
		ConvertTo-Json -InputObject $JSON -Depth 100 | Set-Content -Path ($SettingsJSON.Replace(".json", ("_{0}.json" -f [DateTime]::Now.Ticks))) -Force

		Write-Host "Конвертация завершена успешно!"
	}
	else
	{
		Write-Warning -Message "В файле `"SettingsCE.json`" нет данных!"
	}
}
else
{
	Write-Warning -Message "Файл `"SettingsCE.json`" не найден!"
}
