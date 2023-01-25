using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Interop;
using PalettePlus.Structs;
using PalettePlus.Services;
using PalettePlus.Palettes.Attributes;

namespace PalettePlus.Palettes {
	public class Palette : ICloneable {
		public string Name = "";

		public PaletteConditions Conditions;

		public Dictionary<string, object> ShaderParams = new();

		public Palette(string name = "") {
			Name = name;
		}

		// Palette

		public object Clone() {
			var clone = (Palette)MemberwiseClone();
			clone.ShaderParams = new();
			foreach (var (key, val) in ShaderParams)
				clone.ShaderParams.Add(key, val);
			return clone;
		}

		public unsafe void Apply(Character obj, bool save = false) {
			var model = Model.GetModel(obj);
			if (model != null) {
				model->ApplyPalette(this);
				IpcProvider.PaletteChanged(obj, this);
			}
			if (save) {
				var p = PaletteService.GetCharaPalette(obj).Add(this);
				PaletteService.SetCharaPalette(obj, p);
			}
		}

		public Palette Add(Palette p) {
			var res = (Palette)Clone();
			foreach (var (key, val) in p.ShaderParams)
				res.ShaderParams[key] = val;
			return res;
		}

		// Shader Params

		public void SetShaderParam(string key, object value, bool exists) {
			if (exists)
				ShaderParams[key] = value;
			else
				ShaderParams.Add(key, value);
		}

		public void SetShaderParam(string key, object value)
			=> SetShaderParam(key, value, ShaderParams.ContainsKey(key));

		public void CopyShaderParams(object data) {
			if (data is ModelParams mP) {
				Conditions = PaletteConditions.None;
				if (mP.LeftEyeColor != mP.RightEyeColor)
					Conditions ^= PaletteConditions.Heterochromia;
				if (mP.HairColor != mP.HighlightsColor)
					Conditions ^= PaletteConditions.Highlights;
			}

			var fields = data.GetType().GetFields();
			foreach (var field in fields)
				SetShaderParam(field.Name, field.GetValue(data)!, false);
		}

		public void ApplyShaderParams(ref object data) {
			var fields = data.GetType().GetFields();
			foreach (var field in fields) {
				if (ShaderParams.TryGetValue(field.Name, out var value)) {
					var link = (ConditionalLink?)field.GetCustomAttributes().FirstOrDefault(attr => attr is ConditionalLink);
					if (link != null) {
						var match = (Conditions & link.Conditions) != 0;
						if (!match && !ShaderParams.TryGetValue(link.LinkedTo, out var _))
							continue;
					}

					if (value is JObject j)
						value = j.ToObject(field.FieldType);
					else if (value is double s)
						value = (float)s;

					field.SetValue(data, value);
				}
			}
		}

		// Conversion

		public override string ToString() => JsonConvert.SerializeObject(this);

		public static Palette FromJson(string json) => JsonConvert.DeserializeObject<Palette>(json)!;
	}

	[Flags]
	public enum PaletteConditions {
		None,
		Heterochromia,
		Highlights
	}
}