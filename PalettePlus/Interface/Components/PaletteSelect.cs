using ImGuiNET;

using PalettePlus.Palettes;

namespace PalettePlus.Interface.Components {
	internal static class PaletteSelect {
		internal static bool Draw(string label, string preview, out Palette? selected) {
			var result = false;
			selected = null;

			if (ImGui.BeginCombo(label, preview)) {
				foreach (var palette in PalettePlus.Config.SavedPalettes) {
					if (ImGui.Selectable(palette.Name)) {
						result = true;
						selected = palette;
					}
				}

				ImGui.EndCombo();
			}

			return result;
		}
	}
}