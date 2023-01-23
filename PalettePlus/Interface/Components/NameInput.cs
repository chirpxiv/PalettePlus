using System.Linq;

using ImGuiNET;

using PalettePlus.Palettes;

namespace PalettePlus.Interface.Components {
	internal static class NameInput {
		internal static bool Draw(ref string name, ref string? err, Palette? cur = null) {
			var result = false;

			ImGui.InputTextWithHint("##NewSaveName", "Palette Name", ref name, 100);
			if ((ImGui.IsKeyDown(ImGuiKey.Enter) || ImGui.IsKeyDown(ImGuiKey.KeypadEnter)) && name.Length > 0) {
				var _name = name; // CS1628
				var exists = PalettePlus.Config.SavedPalettes.Any(p => p.Name == _name && p != cur);
				err = exists ? "a palette with this name already exists." : null;

				if (err == null)
					result = true;
			}

			if (err != null) {
				ImGui.PushStyleColor(ImGuiCol.Text, 0xff3030ff);
				ImGui.Text($"Could not save: {err}");
				ImGui.PopStyleColor();
			}

			return result;
		}
	}
}