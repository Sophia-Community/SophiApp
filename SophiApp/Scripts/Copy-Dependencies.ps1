# Copy downloaded dependencies to the created Bin folder
New-Item -Path SophiApp\SophiApp\bin\Release\Bin -ItemType Directory -Force
Get-ChildItem -Path "SophiApp\Binary" | Copy -Destination SophiApp\SophiApp\bin\Release\Bin -Force
