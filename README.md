# SR.ShowOnlinePlayers

SR.ShowOnlinePlayers is a Valheim addon that displays a live list of all players currently connected to your server or local world. Monitor who’s online at a glance without opening the in-game console.

## Features

* 👥 **Real-Time Player List**: Shows each player's username and connection status.
* 🛠️ **Lightweight**: Minimal performance overhead.
* ⚙️ **Customizable Display**: Adjust position and style via config.

## Requirements

* **Valheim** (latest stable version)
* **BepInEx** (5.4.21 or later)
* **Unity Mod Manager** (optional)

## Installation

1. Go to the [Release page](https://github.com/StephanRosin/SR.ShowOnlinePlayers/releases/tag/release) and download the **SR.ShowOnlinePlayers.zip** for the latest release.
2. Extract the ZIP archive.
3. Copy **SR.ShowOnlinePlayers.dll** into your `BepInEx/plugins/` folder.
4. (Optional) Edit the configuration file at `BepInEx/config/SR.ShowOnlinePlayers.cfg` to suit your preferences.
5. Launch Valheim — the online players overlay will appear automatically.

## Usage

* The overlay appears in the top-right corner by default.
* It lists each connected player's character name.
* Refreshes automatically when players join or leave.

## Configuration

On first launch, a config file is created at `BepInEx/config/SR.ShowOnlinePlayers.cfg`. You can modify these settings:

```ini
## Display Settings

## Enable or disable the overlay (default: true)
B:Enabled = true

## X and Y offsets for screen position (pixels)
I:OffsetX = 10
I:OffsetY = 10

## Maximum number of player entries to show
I:MaxEntries = 20

## Font size for player names
I:FontSize = 16
```

Save changes and restart Valheim to apply.
