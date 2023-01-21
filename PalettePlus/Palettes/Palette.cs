using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using PalettePlus.Structs;

namespace PalettePlus.Palettes {
	public class Palette {
		public string Name = "";

		public PaletteConditions Conditions;

		public Dictionary<string, object> ShaderParams = new();

		public Palette(string name = "") {
			Name = name;
		}

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
			var type = data.GetType();

			var fields = type.GetFields();
			foreach (var field in fields) {
				if (ShaderParams.TryGetValue(field.Name, out var value)) {
					if (value is JObject j)
						value = j.ToObject(field.FieldType);
					field.SetValue(data, value);
				}
			}
		}
	}

	[Flags]
	public enum PaletteConditions {
		None,
		Heterochromia,
		Highlights
	}
}