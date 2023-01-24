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
			var input = (ImGui.GetContentRegionAvail().X - (button * 2));

			ImGui.Separator();

			ImGui.Columns(4);
			ImGui.SetColumnWidth(0, button);
			ImGui.SetColumnWidth(1, input * 2/3);
			ImGui.SetColumnWidth(2, input * 1/3);
			ImGui.SetColumnWidth(3, button);

			ImGui.NextColumn();
			var pos = ImGui.GetCursorPosX();
			var avail = ImGui.GetContentRegionAvail().X;
			ImGui.Text("Character");
			ImGui.SameLine();
			ImGui.SetCursorPosX(pos + ImGui.GetStyle().FramePadding.X + (avail * 2/3));
			ImGui.Text("World (Optional)");
			ImGui.NextColumn();
			ImGui.Text("Palette");
			ImGui.NextColumn();
			ImGui.NextColumn();

			ImGui.Separator();

			PersistIndex = 0;

			foreach (var persist in PalettePlus.Config.Persistence.ToArray())
				DrawRow(persist);

			DrawRow(Persist, true);
		}

		private void DrawRow(Persist persist, bool add = false) {
			var redraw = false;

			if (add) {
				ImGui.BeginDisabled(string.IsNullOrEmpty(Persist.Character) || string.IsNullOrEmpty(Persist.PaletteId));
				if (ImGuiComponents.IconButton(PersistIndex, FontAwesomeIcon.Plus)) {
					Persist = new();
					PalettePlus.Config.Persistence.Add(persist);

					add = false;
					redraw = true;
				}
				ImGui.EndDisabled();
			} else {
				redraw |= ImGui.Checkbox($"##PersistEnabled{PersistIndex}", ref persist.Enabled);
			}

			ImGui.NextColumn();
			var avail = ImGui.GetContentRegionAvail().X;
			ImGui.SetNextItemWidth(avail * 2/3);
			ImGui.InputTextWithHint($"##PersistCharaName{PersistIndex}", "Enter character name...", ref persist.Character, 50);
			redraw |= ImGui.IsItemDeactivatedAfterEdit();

			ImGui.SameLine();

			ImGui.SetCursorPosX(ImGui.GetCursorPosX() - ImGui.GetStyle().FramePadding.X);

			ImGui.SetNextItemWidth(avail * 1/3);
			ImGui.InputTextWithHint($"##PersistWorld{PersistIndex}", "Enter world...", ref persist.CharaWorld, 50);
			redraw |= ImGui.IsItemDeactivatedAfterEdit();

			ImGui.NextColumn();
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			if (PaletteSelect.Draw($"##PersistPalette{PersistIndex}", persist.PaletteId == "" ? "Select a palette..." : persist.PaletteId, out var selected)) {
				redraw = true; 
				persist.PaletteId = selected!.Name;
			}

			if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
				persist.PaletteId = "";

			ImGui.NextColumn();
			if (!add && ImGuiComponents.IconButton(PersistIndex, FontAwesomeIcon.Trash)) {
				redraw = true;
				PalettePlus.Config.Persistence.Remove(persist);
			}

			ImGui.NextColumn();

			if (!add && redraw)
				persist.RedrawTargetActor();

			PersistIndex++;
		}
	}
}