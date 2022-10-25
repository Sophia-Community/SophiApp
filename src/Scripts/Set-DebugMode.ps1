# Disable debug mode in Method.cs file
$MethodsContent = Get-Content -Path "src\SophiApp\ViewModels\Methods.cs"
$LineNumber = ($MethodsContent | Select-String -Pattern "DebugMode = true" | Select-Object -Last 1).LineNumber
$FormatedString = $MethodsContent[$LineNumber - 1]
$MethodsContent[$LineNumber - 1] = $FormatedString.Replace("true", "false")
Set-Content -Path "src\SophiApp\ViewModels\Methods.cs" -Value $MethodsContent -Confirm:$false -Encoding UTF8 -Force
