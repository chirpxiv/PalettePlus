﻿using Dalamud.Plugin;

using Dalamud.Game.Command;

using PalettePlus.Services;
using PalettePlus.Interop;
using PalettePlus.Interface;
using PalettePlus.Interface.Windows;

namespace PalettePlus {
	public sealed class PalettePlus : IDalamudPlugin {
		public string Name => "Palette+";
		public string CommandName = "/palette";

		public static Configuration Config { get; internal set; } = null!;

		private Hooks Hooks;

		public PalettePlus(DalamudPluginInterface api) {
			PluginServices.Init(api);

			Configuration.LoadConfig();

			this.Hooks = new Hooks();
			this.Hooks.Init();

			IpcProvider.Init();

			PluginServices.Interface.UiBuilder.DisableGposeUiHide = true;
			PluginServices.Interface.UiBuilder.Draw += PluginGui.Windows.Draw;

			PluginServices.Interface.UiBuilder.OpenConfigUi += ToggleMainWindow;

			PluginServices.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = "Show the Palette+ window."
			});

			PluginServices.ClientState.Logout += OnLogoutEvent;

			PluginServices.Framework.RunOnFrameworkThread(() => {
				foreach (var persist in Config.Persistence)
					persist.RedrawTargetActor();
			});
		}

		public void Dispose() {
			IpcProvider.Dispose();

			this.Hooks.Dispose();

			PluginServices.Interface.UiBuilder.Draw -= PluginGui.Windows.Draw;

			PluginServices.CommandManager.RemoveHandler(CommandName);

			PluginServices.ClientState.Logout -= OnLogoutEvent;

			PluginServices.Framework.RunOnFrameworkThread(PaletteService.RedrawActivePalettes);

			Config.Save();
		}

		private void ToggleMainWindow()
			=> PluginGui.GetWindow<MainWindow>().Toggle();

		private void OnCommand(string _, string arguments)
			=> ToggleMainWindow();

		private void OnLogoutEvent() {
			PluginServices.Log.Verbose("OnLogoutEvent fired, clearing active palettes.");

			PaletteService.ActivePalettes.Clear();
			Hooks.ActorCopy.Clear();

			((MainWindow)PluginGui.GetWindow<MainWindow>()).ActorEdit.ActorList.Selected = null;
		}

		internal static string GetVersion()
			=> typeof(PalettePlus).Assembly.GetName().Version!.ToString(fieldCount: 3);
	}
}
