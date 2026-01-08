# Notes

```terminal
// Works - copies ALSO the actions.json file when compiling
dotnet publish -c Release -p:PublishProfile=FolderProfile
```

---

```terminal
// Works - copies ALSO the actions.json file when compiling
dotnet publish --configuration Release --runtime win-x64 --self-contained true
```

---

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:UseAppHost=true

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:SelfContained=true /p:UseAppHost=true

dotnet publish ./FlowerGUIListener.csproj -c Release --self-contained -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none
