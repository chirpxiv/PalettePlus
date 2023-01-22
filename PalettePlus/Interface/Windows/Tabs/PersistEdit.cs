using ImGuiNET;

using Dalamud.Interface;
using Dalamud.Interface.Components;

using PalettePlus.Palettes;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows.Tabs {
	internal class PersistEdit {
		private int PersistIndex = 0;

		private Persist Persist = new();

		internal void Draw(bool _) {
			var scale = ImGuiHelpers.GlobalScale;

			var button = 30 + (5 * scale);
			var input = (ImGui.GetContentRegionAvail().X - (button * 2)) / 2;

			ImGui.Separator();

			ImGui.Columns(4);
			ImGui.SetColumnWidth(0, button);
			ImGui.SetColumnWidth(1, input);
			ImGui.SetColumnWidth(2, input + 10);
			ImGui.SetColumnWidth(0, button);

			ImGui.NextColumn();
			ImGui.Text("Character");
			ImGui.NextColumn();
			ImGui.Text("Palette");
			ImGui.NextColumn();
			ImGui.NextColumn();

			ImGui.Separator();

			PersistIndex = 0;

			foreach (var persist in PalettePlus.Config.Persistence)
				DrawRow(persist);

			DrawRow(Persist, true);
		}

		private void DrawRow(Persist persist, bool add = false) {
			if (add) {
				if (ImGuiComponents.IconButton(PersistIndex, FontAwesomeIcon.Plus)) {
					PalettePlus.Config.Persistence.Add(persist);
					Persist = new();
				}
			} else {
				ImGui.Checkbox($"##PersistEnabled{PersistIndex}", ref persist.Enabled);
			}

			ImGui.NextColumn();
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			ImGui.InputTextWithHint($"##PersistCharaName{PersistIndex}", "Enter character name...", ref persist.Character, 50);

			ImGui.NextColumn();
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			if (PaletteSelect.Draw($"##PersistPalette{PersistIndex}", persist.PaletteId == "" ? "Select a palette..." : persist.PaletteId, out var selected))
				persist.PaletteId = selected!.Name;

			if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
				persist.PaletteId = "";

			ImGui.NextColumn();
			if (!add && ImGuiComponents.IconButton(PersistIndex, FontAwesomeIcon.Trash)) {
				PalettePlus.Config.Persistence.Remove(persist);
			}

			ImGui.NextColumn();

			PersistIndex++;
		}
	}
}