using System.Collections.Generic;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Extensions;

namespace PalettePlus.Services {
	public enum ApplyOrder {
		PersistFirst,
		StoredFirst // Use when redrawing.
	}

	public static class PaletteService {
		// Shader params

		internal static ParamContainer ParamContainer = new();

		// Param labels

		private static Dictionary<string, string> FieldLabels = new() {
			{ "SkinTone", "Skin Tone" },
			{ "MuscleTone", "Muscle Tone" },
			{ "SkinGloss", "Skin Gloss" },
			{ "LipColor", "Lip Color" },
			{ "HairColor", "Hair Color" },
			{ "HairShine", "Hair Shine" },
			{ "HighlightsColor", "Highlights" },
			{ "LeftEyeColor", "Left Eye" },
			{ "RightEyeColor", "Right Eye" },
			{ "RaceFeatureColor", "Race Feature" },
			{ "FacePaintOffset", "Facepaint Offset" },
			{ "FacePaintWidth", "Facepaint Width" },
			{ "FacePaintColor", "Facepaint Color" }
		};

		public static string GetLabel(string key) => FieldLabels.TryGetValue(key, out var val) ? val : key;

		// Palettes

		internal static Dictionary<Character, Palette> ActivePalettes = new();

		public static Palette GetCharaPalette(Character chara, ApplyOrder order = ApplyOrder.PersistFirst) {
			if (chara.ObjectIndex > 200) {
				var ovw = chara.FindOverworldEquiv();
				if (ovw != null && ovw is Character ovwChara) chara = ovwChara;
			}

			Palette? persists = null;
			foreach (var persist in chara.GetPersists()) {
				if (persists == null)
					persists = persist;
				else
					persists.Add(persist);
			}

			if (!ActivePalettes.TryGetValue(chara, out var stored))
				BuildCharaPalette(chara, out stored, out _);

			switch (order) {
				case ApplyOrder.PersistFirst:
					if (persists == null)
						persists = stored;
					else
						persists.Add(stored);
					return persists;
				case ApplyOrder.StoredFirst:
					if (persists != null)
						stored.Add(persists);
					return stored;
				default:
					return stored;
			}

			//var result = 

			//var active = new Palette();

			/*var persists = chara.GetPersists();

			foreach (var persist in persists)
				active.Add(persist);

			var add = false;
			if (!ActivePalettes.TryGetValue(chara, out var baseVals)) {
				add = true;
				BuildCharaPalette(chara, out baseVals, out _);
			}

			active.Add(baseVals);

			if (add) ActivePalettes[chara] = active;*/

			//return active;
		}

		public static void SetCharaPalette(Character chara, Palette palette) {
			if (chara.ObjectIndex > 200) {
				var ovw = chara.FindOverworldEquiv();
				if (ovw != null && ovw is Character ovwChara) {
					chara = ovwChara;
					palette.Apply(chara);
				}
			}

			ActivePalettes[chara] = palette;
		}

		public static void RemoveCharaPalette(Character chara)
			=> ActivePalettes.Remove(chara);

		public unsafe static void BuildCharaPalette(GameObject obj, out Palette palette, out Palette basePalette, bool contain = false) {
			palette = new();
			basePalette = new();

			var model = Model.GetModel(obj);
			if (model == null) return;

			// TODO: Refactor

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