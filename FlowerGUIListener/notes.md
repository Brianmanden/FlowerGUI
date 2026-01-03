dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:UseAppHost=true

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:SelfContained=true /p:UseAppHost=true

dotnet publish ./FlowerGUIListener.csproj -c Release --self-contained -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none