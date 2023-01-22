using System.Linq;
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
			var result = new List<Palette>();

			var name = actor.Name.ToString();
			foreach (var persist in PalettePlus.Config.Persistence) {
				if (persist.Character == name) {
					var palette = PalettePlus.Config.SavedPalettes.FirstOrDefault(p => p.Name == persist.PaletteId);
					if (palette != null)
						palette.Apply(actor);
				}
			}

			return result;
		}
	}
}