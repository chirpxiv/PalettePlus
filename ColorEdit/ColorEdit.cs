using Dalamud.Plugin;

using Dalamud.Game.Command;

using ColorEdit.Interop;
using ColorEdit.Interface;
using ColorEdit.Interface.Windows;
using Dalamud.Logging;

namespace ColorEdit {
	public sealed class ColorEdit : IDalamudPlugin {
		public string Name => "ColorEdit";
		public string CommandName = "/coloredit";

		public static Configuration Config { get; internal set; } = null!;

		public ColorEdit(DalamudPluginInterface dalamud) {
			Services.Init(dalamud);

			Configuration.LoadConfig();

			Hooks.Init();

			Services.Interface.UiBuilder.DisableGposeUiHide = true;
			Services.Interface.UiBuilder.Draw += PluginGui.Windows.Draw;

			Services.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
				HelpMessage = "/coloredit - Show the ColorEdit window."
			});
		}

		public void Dispose() {
			Hooks.Dispose();

			Services.Interface.UiBuilder.Draw -= PluginGui.Windows.Draw;

			Services.CommandManager.RemoveHandler(CommandName);

			Services.Interface.SavePluginConfig(Config);
		}

		private void OnCommand(string _, string arguments)
			=> PluginGui.GetWindow<MainWindow>().Show();

		internal static string GetVersion()
			=> typeof(ColorEdit).Assembly.GetName().Version!.ToString(fieldCount: 3);
	}
}