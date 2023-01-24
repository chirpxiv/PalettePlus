using System.Linq;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;

using CSGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

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

			var name = obj.Name.ToString();
			var match = !string.IsNullOrEmpty(name) && Character == name;
			if (match && !string.IsNullOrEmpty(CharaWorld) && obj is PlayerCharacter chara) {
				var world = chara.HomeWorld.GameData;

				if (chara.ObjectIndex > 200) {
					var ovw = (PlayerCharacter?)PluginServices.ObjectTable.FirstOrDefault(ch => ch.ObjectIndex < 200 && ch is PlayerCharacter && ch.Name.ToString() == name);
					if (ovw != null) world = ovw.HomeWorld.GameData;
				}

				if (world != null)
					match &= CharaWorld == world.Name;
			}
			return match;
		}

		public GameObject? FindTargetActor() {
			GameObject? result = null;

			foreach (var obj in PluginServices.ObjectTable) {
				if (obj.ObjectIndex < 200) {
					if (result == null && IsApplicableTo(obj))
						result = obj;
					else continue;
				} else if (IsApplicableTo(obj)) {
					result = obj;
					break;
				}
			}

			return result;
		}

		public unsafe void RedrawTargetActor() {
			var tar = FindTargetActor();
			if (tar == null) return;

			var actor = (CSGameObject*)tar.Address;
			actor->DisableDraw();
			actor->EnableDraw();
		}

		public Palette? FindPalette() => PalettePlus.Config.SavedPalettes.FirstOrDefault(p => p.Name == PaletteId);
	}
}