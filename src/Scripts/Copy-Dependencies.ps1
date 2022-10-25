# Copy downloaded dependencies to the created Bin folder
New-Item -Path src\SophiApp\bin\Release\Bin -ItemType Directory -Force
Get-ChildItem -Path "src\Binary" | Copy -Destination src\SophiApp\bin\Release\Bin -Force
