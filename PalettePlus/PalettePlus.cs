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

			PluginServices.Interface.UiBuilder.DisableGposeUiHide = true;
			PluginServices.Interface.UiBuilder.Draw += PluginGui.Windows.Draw;

			PluginServices.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = "/palette - Show the Palette+ window."
			});
		}

		public void Dispose() {
			Hooks.Dispose();

			PluginServices.Interface.UiBuilder.Draw -= PluginGui.Windows.Draw;

			PluginServices.CommandManager.RemoveHandler(CommandName);

			Config.Save();
		}

		private void OnCommand(string _, string arguments)
			=> PluginGui.GetWindow<MainWindow>().Show();

		internal static string GetVersion()
			=> typeof(PalettePlus).Assembly.GetName().Version!.ToString(fieldCount: 3);
	}
}