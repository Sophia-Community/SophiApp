$ReleaseBinDir = "$(Split-Path -Path $PSScriptRoot -Parent)\src\bin\Release\Bin"
New-Item -Path (Split-Path -Path $ReleaseBinDir -Parent) -Name Bin -ItemType Directory -Force

# Copy downloaded dependencies to the created Bin folder
Get-ChildItem -Path "SophiApp\Binary" | Copy -Destination $ReleaseBinDir -Force
