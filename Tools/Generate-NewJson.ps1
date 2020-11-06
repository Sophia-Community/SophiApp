Clear-Host
$Error.Clear()

#region Variables
$sourceFilePath = "{0}\Settings\SettingsCE_copy.json"-f (Split-Path -Path (Get-Location).Path)
$jsonFilePath = "{0}\Settings\SettingsCE_v{1}.json"-f (Split-Path -Path (Get-Location).Path), (Get-Date -Format "dd.MM.yyyy")
$jsonData = New-Object System.Collections.ArrayList($null)
#endregion

#region Functions
function Set-TagFromId {  

  [CmdletBinding()]
  param
  (
    [Parameter(Mandatory=$True)]    
	[ValidateNotNull()]
    [string]$Id
  )
  
  $id = $Id.Split("x")[0]
  switch($id)
  {  
  	1 {return "Privacy"}
	2 {return "Ui"}
	3 {return "OneDrive"}
	4 {return "System"}
	5 {return "StartMenu"}
	6 {return "Uwp"}
	7 {return "Game"}
	8 {return "TaskSheduler"}
	9 {return "Security"}
	10 {return "ContextMenu"}
  
  }
}
#endregion

if (Test-Path -Path $sourceFilePath)
{
	$parsedJson = Get-Content -Path $sourceFilePath -Encoding UTF8 | Out-String | ConvertFrom-Json
	$parsedJson | %{
	
		$header = New-Object 'System.Collections.Generic.Dictionary[string,string]'
		[Void]$header.Add("RU", $_.HeaderRu)
		[Void]$header.Add("EN", $_.HeaderEn)
		
		$description = New-Object 'System.Collections.Generic.Dictionary[string,string]'
		[Void]$description.Add("RU", $_.DescriptionRu)
		[Void]$description.Add("En", $_.DescriptionEn)
		
		$tag = Set-TagFromId -Id $_.Id
		
		$props = [ordered]@{ Id = $_.Id
							 Header = $header							  
							 Description = $description							  
							 Type = $_.Type
							 Tag = $tag }
					
		$jsonObject = New-Object -TypeName PSObject -Property $props
		[Void]$jsonData.Add($jsonObject)
	}
	
	if (Test-Path -Path $jsonFilePath)
	{
		try
		{
			Remove-Item -Path $jsonFilePath -Force -Confirm:$false -ErrorAction Stop
			Write-Warning -Message ("Файл удалён:""{0}"""-f $jsonFilePath)			
		}
		
		catch
		{
			Write-Warning -Message ("Не удалось удалить файл:""{0}"""-f $jsonFilePath)
		}
	}
	
	try
	{
		$jsonData | ConvertTo-Json | Out-File -FilePath $jsonFilePath -Encoding UTF8 -Confirm:$false -Force -ErrorAction Stop
		"Данные сохранены в файл:""{0}"""-f $jsonFilePath
	}
	
	catch
	{
		"Не удалось сохранить данные в файл:""{0}"""-f $jsonFilePath
	}
}

else
{
	Write-Warning -Message "Не найден исходный файл JSON!"
}
