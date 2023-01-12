using System;
using System.Collections.Generic;

using PalettePlus.Structs;

namespace PalettePlus.Palettes {
	public class Palette {
		public PaletteConditions Conditions;

		public Dictionary<string, object> ShaderParams = new();

		public void SetShaderParam(string key, object value, bool exists) {
			if (exists)
				ShaderParams[key] = value;
			else
				ShaderParams.Add(key, value);
		}

		public void SetShaderParam(string key, object value)
			=> SetShaderParam(key, value, ShaderParams.ContainsKey(key));

		public void CopyShaderParams(ModelParams data) {
			Conditions = PaletteConditions.None;
			if (data.LeftEyeColor != data.RightEyeColor)
				Conditions ^= PaletteConditions.Heterochromia;
			if (data.HairColor != data.HighlightsColor)
				Conditions ^= PaletteConditions.Highlights;

			var fields = typeof(ModelParams).GetFields();
			foreach (var field in fields)
				SetShaderParam(field.Name, field.GetValue(data)!, false);
		}

		public void ApplyShaderParams(ref object data) {
			if (data is ModelParams == false) return;
			foreach (var (name, value) in ShaderParams) {
				var field = typeof(ModelParams).GetField(name);
				if (field != null) field.SetValue(data, value);
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