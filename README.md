# Title Renamed

A FFXIV Dalamud plugin to rename a title displayed on the nameplate to something else.

Optionally, hide a specific title from the nameplate.

To be used with [XIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher).

(NOTE: this plugin only make changes to how title is displayed on the nameplate. 
It does not changes anything else regarding the title.
So in all other places the titles will remain unchanged.)


## How to Install

[XIVLauncher](https://github.com/goatcorp/FFXIVQuickLauncher) is required to install and run the plugin.

Add my [Dalamud plugin repo](https://github.com/yomishino/DalamudPlugins) to Dalamud's Custom Plugin Repositories,
see [here](https://github.com/yomishino/DalamudPlugins#readme) for details.

Once added, look for the plugin "Title Renamed" in Plugin Installer's available plugins.


## How to Use

Open Title Renamed Configuration window through the Plugin Installer, 
or enter in chat the text command `/titlerenamed config` to open it.

Add, modify or remove title renaming entries in Configuration.

Once entries are added and enabled, as long as the plugin is enabled, 
the titles in the list will be automatically renamed (or hidden) on the nameplate.

### Enabling/Disabling the Plugin

Check the "__Enable plugin__" to enable the plugin in the plugin's configuration window.

Also check "__Auto enable plugin on start__" if you would like the plugin to be auto enabled everytime you start the game.
Otherwise the plugin will not be enabled next time you start the game.

Additionally, you can enable/disable for individual entries 
by checking/unchecking the box before each individual entry.

### Setting Up the Title Rename List

In the configuration window, you should see the __Title Rename List__ as a table,
where all added entries will be listed in it.

The last row of the list has input text boxes and so on for adding a new entry.

If you can't see the list, click on the "__▶ Title Rename List__" to make it appear.

You can add, remove or configure entries in the list here.
Effect of changes should take place immediately, although sometimes you may need to wait a bit 
or move your camera around etc. to let the game update the nameplates.

Click "__Save Title Rename List__" button to have the changes to the list saved immediately to the configuration file.

### Adding/Removing Entries in Title Rename List 

_To add an entry_, type in the original title in the first input box,
and type in the new title, that is, the text that you want the original title to be renamed to, in the second input text box.
(The new title can be empty, especially if you just want to add an entry to hide a specific title.)
Change the additional settings for the entry (see [Configuraing Rename Entries](#configuring-rename-entries) below) if applicable.
Then click the "+" button on the right.

_To remove an entry_, click the "-" button to the right of that entry.

### Configuring Rename Entries

You can configure each entry on the following options:
 - __Enable/Disable__ individual entry: Check/uncheck the box to the left of each entry to enable/disable just for that entry;
 - __Prefix/Suffix__: Choose whether to display the title as a prefix or suffix title; 
 - __Display__: Check/uncheck the box to set whether to display the title on the nameplates.

 These options can be changed after the entries have been added to the Title Rename List.

### Available Text Commands and Arguments

- `/titlerenamed`: Toggle whether to enable the plugin
- `/titlerenamed on`: Enable the plugin
- `/titlerenamed off`: Disable the plugin
- `/titlerenamed config`: Open the Title Renamed configuration window


## Pro Tips

- If you only want titles to be renamed on demand, 
	uncheck the "__Auto enable plugin on start__" setting 
	and enable/disable the plugin through configuration window or text commands instead.
- To simply hide a specific title from the nameplates,
	add that title as an entry and _uncheck_ "__Display__".
