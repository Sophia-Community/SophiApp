# Remove comments in .cs files
$MethodsContent = Get-Content -Path "src\SophiApp\Helpers\StartupConditionsHelper.cs"
$LineNumber = ($MethodsContent | Select-String -Pattern "//new NewVersionCondition()" | Select-Object -Last 1).LineNumber
$FormatedString = $MethodsContent[$LineNumber - 1]
$MethodsContent[$LineNumber - 1] = $FormatedString.Replace("//", $null)
Set-Content -Path "src\SophiApp\Helpers\StartupConditionsHelper.cs" -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force

