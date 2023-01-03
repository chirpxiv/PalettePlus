using Dalamud.Plugin;

using Dalamud.Game.ClientState.Objects.Types;

using ColorEdit.Interface;
using ColorEdit.Interface.Windows;

namespace ColorEdit {
	public sealed class ColorEdit : IDalamudPlugin {
		public string Name => "ColorEdit";
		public string CommandName = "/coloredit";

		public ColorEdit(DalamudPluginInterface dalamud) {
			Services.Init(dalamud);

			Services.Interface.UiBuilder.DisableGposeUiHide = true;
			Services.Interface.UiBuilder.Draw += PluginGui.Windows.Draw;

			PluginGui.GetWindow<MainWindow>().Show();
		}

		public void Dispose() {
			Services.Interface.UiBuilder.Draw -= PluginGui.Windows.Draw;
		}

		internal unsafe static GameObject? GetPlayer()
			=> Services.ClientState.LocalPlayer;
	}
}