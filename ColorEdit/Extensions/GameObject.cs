using Dalamud.Game.ClientState.Objects.Types;

using ColorEdit.Interop;
using ColorEdit.Structs;

namespace ColorEdit.Extensions {
	internal static class GameObjectExtensions {
		internal unsafe static DrawParams* UpdateColors(this GameObject actor) {
			var model = Model.GetModel(actor);
			if (model == null) return null;

			Hooks.UpdateColorsHook.Original(model);

			return model->GetColorData();
		}
	}
}