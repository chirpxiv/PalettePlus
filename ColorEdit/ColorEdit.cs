using Dalamud.Plugin;

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

			PluginGui.GetWindow<MainWindow>().Show();
		}

		public void Dispose() {
			Hooks.Dispose();

			Services.Interface.UiBuilder.Draw -= PluginGui.Windows.Draw;
		}
	}
}