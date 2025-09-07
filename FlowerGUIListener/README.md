# FlowerGUI Listener

Et moderne WPF-program der giver hurtig adgang til praktiske funktioner via globale genveje.

## ✨ Funktioner

- **Global Hotkey**: Aktiver FlowerGUI overalt på systemet med Ctrl + højreklik
- **Moderne GUI**: Elegant design med moderne WPF styling og animationer
- **Hurtige Handlinger**:
  - 📝 Opret notater direkte på skrivebordet
  - 📷 Tag skærmbilleder og gem automatisk
  - 📋 Vis og håndter udklipsholder-indhold
  - 🔍 Åbn Windows søgning hurtigt
- **System Tray Integration**: Diskret baggrundskørsel med kontekstmenu
- **Indstillinger**: Konfigurerbare mapper og præferencer
- **Aktivitetsoversigt**: Hold styr på dine seneste handlinger

## 🚀 Sådan bruges det

1. **Start**: Programmet kører i baggrunden og viser et ikon i system tray
2. **Aktiver**: Tryk Ctrl + højreklik hvor som helst på skærmen
3. **Vælg handling**: Klik på en af knapperne for at udføre hurtige opgaver
4. **Luk**: Tryk Esc eller klik på X-knappen

## 🎮 Genvejstaster

- `Ctrl + Højreklik`: Åbn FlowerGUI
- `Esc`: Luk FlowerGUI
- `Ctrl + H`: Vis hjælp
- Dobbeltklik på tray-ikon: Åbn FlowerGUI

## 🏗️ Arkitektur

Programmet er bygget med moderne C# og WPF og følger SOLID principper:

### Services
- **GlobalHookManager**: Håndterer systemgenveje og Windows hooks
- **TrayIconManager**: Administrerer system tray funktionalitet

### Models
- **Settings**: Konfigurationsstyring med JSON-persistering

### Windows
- **MainWindow**: Original WPF vindue (skjult)
- **FlowerGUIWindow**: Hovedfunktionalitetsvindue med moderne UI

## 📁 Filstruktur

```
FlowerGUIListener/
├── Services/
│   ├── GlobalHookManager.cs    # Windows hooks og hotkeys
│   └── TrayIconManager.cs      # System tray integration
├── Models/
│   └── Settings.cs             # Konfigurationsstyring
├── Windows/
│   ├── FlowerGUIWindow.xaml    # Hoved-GUI design
│   └── FlowerGUIWindow.xaml.cs # GUI funktionalitet
├── App.xaml                    # WPF applikation definition
├── App.xaml.cs                 # Applikationslogik
├── MainWindow.xaml             # Original vindue (skjult)
└── MainWindow.xaml.cs          # Code-behind
```

## ⚙️ Indstillinger

Indstillinger gemmes automatisk i:
`%APPDATA%\\FlowerGUI\\settings.json`

Konfigurerbare optioner:
- Hotkey kombination
- Start med Windows
- Notifikationer
- Noter-mappe
- Skærmbilleder-mappe

## 🔧 Udvikling

### Forudsætninger
- .NET 8.0 SDK
- Windows 10/11
- Visual Studio eller VS Code

### Build
```bash
dotnet build
```

### Kør
```bash
dotnet run
```

## 🛠️ Forbedringer implementeret

✅ **Arkitekturrefaktorering**: Opdelt i separate service-klasser  
✅ **Fejlhåndtering**: Robust exception handling og graceful degradation  
✅ **Moderne UI**: WPF styling med animationer og skygger  
✅ **Ressourcestyring**: Proper disposal og memory management  
✅ **Funktionalitet**: Erstattede placeholder med rigtige funktioner  
✅ **Konfiguration**: JSON-baseret indstillingssystem  

## 📝 Licens

Dette projekt er udviklet som en del af FlowerGUI-systemet og er til intern brug.

## 🐛 Fejlrapportering

Hvis du oplever problemer:
1. Tjek om programmet kører som administrator (hvis nødvendigt)
2. Kontroller at Windows Defender ikke blokerer hooking-funktionaliteten
3. Se debug-output i Visual Studio Output-vinduet

---

*Lavet med ❤️ og moderne C# + WPF*
