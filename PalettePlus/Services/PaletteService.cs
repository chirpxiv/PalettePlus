using System;
using System.Linq;
using System.Collections.Generic;

using Dalamud.Logging;
using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Extensions;
using PalettePlus.Interop;

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

		internal static Dictionary<(string, string), Palette> ActivePalettes = new();

		public static Palette GetCharaPalette(Character chara, ApplyOrder order = ApplyOrder.PersistFirst) {
			if (chara.ObjectIndex >= 200 && chara.ObjectIndex < 240) {
				var ovw = chara.FindOverworldEquiv();
				if (ovw != null && ovw is Character ovwChara) chara = ovwChara;
			}

			var key = chara.GetNameAndWorld();

			Palette? persists = null;
			foreach (var persist in chara.GetPersists())
				persists = persists == null ? persist : persists.Add(persist);

			var store = false;
			if (!ActivePalettes.TryGetValue(key, out var stored)) {
				BuildCharaPalette(chara, out stored, out _);
				store = persists != null || stored.ShaderParams.Count > 0;
			}
			
			var result = order switch {
				ApplyOrder.PersistFirst => persists == null ? stored : persists.Add(stored),
				ApplyOrder.StoredFirst => persists == null ? stored : stored.Add(persists),
				_ => stored
			};

			if (store) ActivePalettes[key] = result;

			return result;
		}

		public static void SetCharaPalette(Character chara, Palette palette) {
			if (chara.ObjectIndex >= 200 && chara.ObjectIndex < 240) {
				var ovw = chara.FindOverworldEquiv();
				if (ovw != null && ovw is Character ovwChara)
					chara = ovwChara;
			}

			palette.Apply(chara);
			ActivePalettes[chara.GetNameAndWorld()] = palette;
		}

		public static void RemoveCharaPalette(Character chara) {
			if (ActivePalettes.Remove(chara.GetNameAndWorld())) {
				IpcProvider.PaletteChanged(chara, null);
			}
		}

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

		public static void RedrawActivePalettes() {
			if (PluginServices.Framework.IsFrameworkUnloading || !PluginServices.ClientState.IsLoggedIn)
				return;

			foreach ((string,string) key in ActivePalettes.Keys) {
				try {
					var chara = PluginServices.ObjectTable.FirstOrDefault(a => {
						if (a is Character chara)
							return chara.GetNameAndWorld() == key;
						return false;
					});

					if (chara != null) chara.Redraw();
				} catch (Exception e) {
					PluginLog.Error($"Failed to redraw actor: {key}", e);
				}
			}
		}
	}
}