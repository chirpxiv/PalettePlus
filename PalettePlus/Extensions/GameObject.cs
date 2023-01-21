using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Interop;
using PalettePlus.Structs;

namespace PalettePlus.Extensions {
	internal static class GameObjectExtensions {
		internal unsafe static ModelParams* UpdateColors(this GameObject actor) {
			var model = Model.GetModel(actor);
			if (model == null) return null;

			Hooks.UpdateColorsHook.Original(model);

			return model->GetModelParams();
		}
	}
}