New-Item -Path SophiApp\SophiApp\bin\Release\Bin -ItemType Directory -Force
# Copy downloaded dependencies to the created Bin folder
Get-ChildItem -Path "SophiApp\Binary" | Copy -Destination SophiApp\SophiApp\bin\Release\Bin -Force
