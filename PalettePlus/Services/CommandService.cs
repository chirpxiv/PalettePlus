using System;
using System.Linq;
using System.Collections.Generic;

using Dalamud.Logging;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;

using PalettePlus.Interface;

namespace PalettePlus.Services; 

// ReSharper disable once ClassNeverInstantiated.Global
public class CommandService : IDisposable {
	private readonly ICommandManager _cmdManager;
	private readonly GuiService _guiService;

	private readonly Dictionary<string, CommandInfo> Commands = new();
	
	// Constructor
	
	public CommandService(ICommandManager _cmdManager, GuiService guiService) {
		this._cmdManager = _cmdManager;
		this._guiService = guiService;
		
		this.AddHandler("/palette", new CommandInfo(OnCommand) {
			HelpMessage = "Toggle the Palette+ window."
		});
	}

	private void AddHandler(string name, CommandInfo cmd) {
		PluginLog.Verbose($"Registering command handler for '{name}'");
		if (this._cmdManager.AddHandler(name, cmd))
			this.Commands.Add(name, cmd);
		else
			throw new Exception($"Failed to register command '{name}'.");
	}
	
	// Main command handler

	private void OnCommand(string _cmd, string _args)
		=> this._guiService.ToggleMainWindow();
	
	// Disposal

	public void Dispose() {
		PluginLog.Verbose("Disposing of commands...");
		this.Commands.Keys.ToList().ForEach(RemoveHandler);
		this.Commands.Clear();
	}

	private void RemoveHandler(string name) {
		if (this._cmdManager.RemoveHandler(name))
			PluginLog.Verbose($"Removed command handler for '{name}'.");
		else
			PluginLog.Warning($"Failed to remove command handler for '{name}'!");
	}
}