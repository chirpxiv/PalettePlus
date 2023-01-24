using System.Collections.Generic;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Interop;
using PalettePlus.Structs;
using PalettePlus.Palettes;

namespace PalettePlus.Extensions {
	internal static class GameObjectExtensions {
		internal unsafe static ModelParams* UpdateColors(this GameObject actor) {
			var model = Model.GetModel(actor);
			if (model == null) return null;

			Hooks.UpdateColors(model);

			return model->GetModelParams();
		}

		internal static List<Palette> GetPersists(this GameObject actor) {
			var results = new List<Palette>();

			foreach (var persist in PalettePlus.Config.Persistence) {
				if (!persist.Enabled || !persist.IsApplicableTo(actor)) continue;

				var palette = persist.FindPalette();
				if (palette != null) results.Add(palette);
			}

			return results;
		}

		internal unsafe static bool IsValidForPalette(this GameObject obj) {
			var actor = (Actor*)obj.Address;
			return !(actor == null || actor->ModelId != 0 || actor->GetModel() == null);
		}
	}
}