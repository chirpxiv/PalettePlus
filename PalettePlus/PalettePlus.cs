using Dalamud.Plugin;

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

		public PalettePlus(DalamudPluginInterface dalamud) {
			PluginServices.Init(dalamud);

			Configuration.LoadConfig();

			Hooks.Init();

			IpcProvider.Init();

			PluginServices.Interface.UiBuilder.DisableGposeUiHide = true;
			PluginServices.Interface.UiBuilder.Draw += PluginGui.Windows.Draw;

			PluginServices.Interface.UiBuilder.OpenConfigUi += ToggleMainWindow;

			PluginServices.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = "/palette - Show the Palette+ window."
			});
		}

		public void Dispose() {
			IpcProvider.Dispose();

			Hooks.Dispose();
			PaletteService.RedrawActivePalettes();

			PluginServices.Interface.UiBuilder.Draw -= PluginGui.Windows.Draw;

			PluginServices.CommandManager.RemoveHandler(CommandName);

			Config.Save();
		}

		private void ToggleMainWindow()
			=> PluginGui.GetWindow<MainWindow>().Toggle();

		private void OnCommand(string _, string arguments)
			=> ToggleMainWindow();

		internal static string GetVersion()
			=> typeof(PalettePlus).Assembly.GetName().Version!.ToString(fieldCount: 3);
	}
}