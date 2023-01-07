using Dalamud.Plugin;

using Dalamud.Game.Command;

using ColorEdit.Interop;
using ColorEdit.Interface;
using ColorEdit.Interface.Windows;

namespace ColorEdit {
	public sealed class ColorEdit : IDalamudPlugin {
		public string Name => "ColorEdit";
		public string CommandName = "/coloredit";

		public ColorEdit(DalamudPluginInterface dalamud) {
			Services.Init(dalamud);

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
		}

		private void OnCommand(string _, string arguments) {
			PluginGui.GetWindow<MainWindow>().Show();
		}
	}
}