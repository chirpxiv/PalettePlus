using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;

using PalettePlus.Interface.Windows.Tabs;

namespace PalettePlus.Interface.Windows {
	public class MainWindow : Window {
		private ActorEdit ActorEdit = new();
		private PaletteEdit PaletteEdit = new();

		// Window

		public MainWindow() : base(
			"Palette+"
		) {
			SizeConstraints = new WindowSizeConstraints {
				MinimumSize = new Vector2(470, 210),
				MaximumSize = ImGui.GetIO().DisplaySize
			};
		}

		public override void Draw() {
			if (ImGui.BeginTabBar("ColorEdit Tabs")) {
				DrawTab("Edit Players", ActorEdit.Draw);
				DrawTab("Saved Palettes", PaletteEdit.Draw);
				DrawTab("Persistence", (bool _) => {});
				ImGui.EndTabBar();
			}
		}

		// Tabs

		private string CurrentTab = "";

		private void DrawTab(string label, Action<bool> callback) {
			if (ImGui.BeginTabItem(label)) {
				var switched = CurrentTab != label;
				if (switched)
					CurrentTab = label;

				callback.Invoke(switched);

				ImGui.EndTabItem();
			}
		}
	}
}