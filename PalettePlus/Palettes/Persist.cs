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

		public unsafe bool IsApplicableTo(GameObject obj) {
			if (!obj.IsValidForPalette()) return false;

			var match = Character == obj.Name.ToString();
			if (match && CharaWorld != "" && obj is PlayerCharacter chara) {
				if (chara.HomeWorld.GameData != null)
					match &= CharaWorld == chara.HomeWorld.GameData.Name;
			}
			return match;
		}

		public GameObject? FindApplicableActor() {
			if (!Enabled) return null;

			foreach (var obj in PluginServices.ObjectTable) {
				if (IsApplicableTo(obj))
					return obj;
			}

			return null;
		}

		public Palette? FindPalette() => PalettePlus.Config.SavedPalettes.FirstOrDefault(p => p.Name == PaletteId);
	}
}