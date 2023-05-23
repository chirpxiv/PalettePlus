using System.Linq;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;

using PalettePlus.Services;
using PalettePlus.Extensions;

namespace PalettePlus.Palettes {
	public class Persist {
		public bool Enabled = true;
		public string Character = "";
		public string CharaWorld = "";
		public string PaletteId = "";

		public unsafe bool IsApplicableTo(Character obj) {
			if (!obj.IsValidForPalette()) return false;

			var name = obj.Name.ToString();
			var match = !string.IsNullOrEmpty(name) && Character.TrimAndSquash().Equals(name.TrimAndSquash(), System.StringComparison.OrdinalIgnoreCase);
			if (match && !string.IsNullOrEmpty(CharaWorld) && obj is PlayerCharacter chara) {
				var world = chara.HomeWorld.GameData;

				if (chara.ObjectIndex > 200 && chara.ObjectIndex < 241) {
					var ovw = (PlayerCharacter?)chara.FindOverworldEquiv();
					if (ovw != null) world = ovw.HomeWorld.GameData;
				}

				if (world != null)
					match &= CharaWorld == world.Name;
			}
			return match;
		}

		public Character? FindTargetActor() {
			Character? result = null;

			foreach (var obj in PluginServices.ObjectTable) {
				if (obj is not Character chara) continue;
				
				if (chara.ObjectIndex < 200) {
					if (result == null && IsApplicableTo(chara))
						result = chara;
				} else if (IsApplicableTo(chara)) {
					result = chara;
					break;
				}
			}

			return result;
		}

		public void RedrawTargetActor() {
			var tar = FindTargetActor();
			if (tar == null) return;
			tar.Redraw();
		}

		public Palette? FindPalette() => PalettePlus.Config.SavedPalettes.FirstOrDefault(p => p.Name == PaletteId);
	}
}