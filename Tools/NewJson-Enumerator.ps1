Clear-Host
$Error.Clear()

#region Variables
$sourceFilePath = "{0}\Settings\ControlsData.json"-f (Split-Path -Path (Get-Location).Path)
$jsonFilePath = "{0}\Settings\ControlsData_v{1}.json"-f (Split-Path -Path (Get-Location).Path), (Get-Date -Format "dd.MM.yyyy")
$jsonData = New-Object System.Collections.ArrayList($null)
#endregion

#region Functions
function Set-IdFromTag {  

  [CmdletBinding()]
  param
  (
    [Parameter(Mandatory=$True)]    
	[ValidateNotNull()]
    [string]$Id	
  )
  
  $data = $Id.Split("x")
  $id = $data[0]
  $counter = $data[1] - 1
  
  switch($id)
  {  
  	1 {return 100 + $counter}
	2 {return 200 + $counter}
	3 {return 300 + $counter}
	4 {return 400 + $counter}
	5 {return 500 + $counter}
	6 {return 600 + $counter}
	7 {return 700 + $counter}
	8 {return 800 + $counter}
	9 {return 900 + $counter}
	10 {return 1000 + $counter}  
  }
}
#endregion

if (Test-Path -Path $sourceFilePath)
{
	$parsedJson = Get-Content -Path $sourceFilePath -Encoding UTF8 | Out-String | ConvertFrom-Json
	for ($i=0;$i -lt $parsedJson.Count;$i++)
	{
		$id = Set-IdFromTag -Id $parsedJson[$i].Id		
		$props = [ordered]@{ Id = $id
							 LocalizedHeader = $parsedJson[$i].LocalizedHeader
							 LocalizedDescription = $parsedJson[$i].LocalizedDescription							  
							 Type = $parsedJson[$i].Type
							 Tag = $parsedJson[$i].Tag }
					
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
