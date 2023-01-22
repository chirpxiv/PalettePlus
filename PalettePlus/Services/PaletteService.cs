using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes;

namespace PalettePlus.Services {
	public static class PaletteService {
		public static ParamContainer ParamContainer = new();

		public unsafe static void GetCharaPalette(GameObject obj, out Palette palette, out Palette basePalette, bool contain = false) {
			palette = new();
			basePalette = new();

			var model = Model.GetModel(obj);
			if (model == null) return;

			// TODO: Refactor

			palette.ShaderParams.Clear();

			var mP = model->GetModelParams();
			if (mP != null)
				palette.CopyShaderParams(*mP);

			var dP = model->GetDecalParams();
			if (dP != null)
				palette.CopyShaderParams(*dP);

			var vals = model->GenerateColorValues();
			basePalette.CopyShaderParams(vals.Model);
			basePalette.CopyShaderParams(vals.Decal);
			if (contain) ParamContainer = vals;

			foreach (var (key, value) in palette.ShaderParams) {
				if (value.Equals(basePalette.ShaderParams[key]))
					palette.ShaderParams.Remove(key);
			}
		}
	}
}