# Running FlowerGUI as a Service (Minimized on Startup)

This document summarizes the discussion regarding making the FlowerGUI program run as a background application that starts minimized/hidden when the computer boots up and listens for events.

## Key Components:

1.  **Running on Startup:** How to make the application launch automatically when the user logs in.
2.  **Starting Minimized/Hidden:** How to ensure the main window doesn't appear on startup but runs silently in the background.
3.  **Background Listening & UI Control:** How the application listens for hotkeys while hidden and how the user can interact with it.

---

## 1. Running on Startup

There are two primary methods:

*   **Registry Method (Recommended):**
    *   **How it works:** Code within the application adds an entry to the Windows Registry at:
        `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`
    *   **Entry details:** The entry's name would be the application name (e.g., "FlowerGUIListener"), and its value would be the full path to the executable (e.g., `"C:\Path\To\FlowerGUIListener.exe"`).
    *   **Benefit:** This is the standard, more robust, and programmatically controllable way to achieve startup execution. It allows for options like adding a "Run on Startup" checkbox in the application's settings.

*   **Startup Folder Method (Simpler, Manual):**
    *   **How it works:** A shortcut to the application's executable is manually placed in the Windows Startup folder (accessible via `shell:startup`).
    *   **Benefit:** Very easy for a user to configure manually.
    *   **Drawback:** Less programmatic control and less "professional" for a distributed application.

---

## 2. Starting Minimized or Hidden

This involves modifying the application's internal behavior at launch.

*   **Modify `App.xaml.cs`'s `OnStartup` event:**
    *   The application would still create instances of the main window (`MainWindow`) and the flower UI window (`FlowerGUIWindow`).
    *   Crucially, these windows would *not* be explicitly shown immediately. Instead, their visibility would be set to `Visibility.Hidden` or `window.Hide()` would be called during startup. This ensures the application starts without a visible UI, running silently in the background.

---

## 3. Running as a Background "Service" (System Tray Application)

This is the most user-friendly approach for desktop utilities that need to run in the background. Your project's existing `TrayIconManager.cs` is perfectly suited for this.

*   **On Application Startup:**
    *   The application launches (as configured via the Registry entry).
    *   The main UI windows are created but remain hidden.
*   **System Tray Icon:**
    *   The `TrayIconManager` immediately activates and places an icon in the Windows system tray (notification area).
    *   This icon is the user's primary interface to the hidden application.
*   **Tray Icon Context Menu:**
    *   Right-clicking the tray icon reveals a context menu with essential options:
        *   **"Show Flower" (or "Open"):** Displays the hidden `FlowerGUIWindow`.
        *   **"Settings":** (Optional) Opens a settings window where the user can configure preferences, including the "Run on Startup" option.
        *   **"Exit":** Gracefully shuts down the entire application, removes the tray icon, and stops all background processes (like the hotkey listener).
*   **Background Listening (`GlobalHookManager`):**
    *   The `GlobalHookManager` starts running on application launch, continuously monitoring for the defined global hotkey (e.g., Ctrl + Right-click).
    *   When the hotkey is detected, it triggers the display of the hidden `FlowerGUIWindow` (using its `ShowAt` method).
*   **User Interaction Flow:**
    *   The user presses the hotkey -> FlowerGUI appears.
    *   The user interacts with FlowerGUI.
    *   FlowerGUI hides itself (e.g., on `Escape` key or loss of focus).
    *   The application remains in the background, only accessible via the hotkey or tray icon.

This combined approach provides a seamless user experience, allowing the utility to be always available without being intrusive.
