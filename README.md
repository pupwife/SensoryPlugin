# Sensory

A Baby Sensory, Cocomelon-esque plugin for Final Fantasy XIV.

Bring calming, colorful, and engaging baby sensory visuals to your FFXIV experience! This plugin provides soothing visual effects inspired by baby sensory videos and Cocomelon-style animations to help create a relaxing atmosphere while you play.

## Features

* Calming visual effects and animations
* Cocomelon-inspired colorful displays
* Customizable settings for your preferred experience
* Easy-to-use slash command interface
* Main UI window for visual display
* Configuration window for personalization

## Prerequisites

Sensory requires the following to be installed and configured:

* XIVLauncher, FINAL FANTASY XIV, and Dalamud have all been installed and the game has been run with Dalamud at least once.
* XIVLauncher is installed to its default directories and configurations.
  * If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable.
* A .NET Core 8 SDK has been installed and configured, or is otherwise available. (In most cases, the IDE will take care of this.)

## Building

1. Open up `Sensory.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `Sensory/bin/x64/Debug/Sensory.dll` (or `Release` if appropriate.)

## Installation

### Easy Method (Recommended)

1. Open Dalamud Plugin Installer (`/xlplugins` in chat or `xlplugins` in console)
2. Go to **Settings** → **Experimental** → **Custom Plugin Repositories**
3. Click **Add** and paste this URL:
   ```
   https://raw.githubusercontent.com/pupwife/SensoryPlugin/master/repo.json
   ```
4. Click **Save** and return to the **Available Plugins** tab
5. Search for "Sensory" and click **Install**
6. The plugin will automatically update when new versions are released!

### For Development

If you want to build from source:

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
   * In here, go to `Experimental`, and add the full path to the `Sensory.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
   * In here, go to `Dev Tools > Installed Dev Plugins`, and the `Sensory` plugin should be visible. Enable it.
3. You should now be able to use `/sensory` (chat) or `sensory` (console) to open the main window!

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load your plugin on startup through the Plugin Installer.

## Usage

* Use `/sensory` in chat or `sensory` in the Dalamud Console to toggle the main visual window.
* Use `/sensoryconfig` in chat or `sensoryconfig` in the Dalamud Console to open the configuration window.
* You can also access the configuration window through the Plugin Installer (`/xlplugins`) by clicking the gear icon next to the Sensory plugin.

## Development

This plugin is built using the [Dalamud Plugin API](https://dalamud.dev) and follows the standard Dalamud plugin structure. For more information about developing Dalamud plugins, check out the [Dalamud Developer Docs](https://dalamud.dev).

## License

This project is licensed under the AGPL-3.0-or-later license. See the [LICENSE.md](LICENSE.md) file for details.

## Author

Created by **pupwife**

---

*Enjoy your calming sensory experience in Eorzea!* 🌈✨
