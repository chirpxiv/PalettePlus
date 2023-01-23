using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes.Attributes;

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

		public unsafe void Apply(GameObject obj) {
			//if (obj.ObjectKind)

			var model = Model.GetModel(obj);
			if (model != null)
				model->ApplyPalette(this);
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
	}

	[Flags]
	public enum PaletteConditions {
		None,
		Heterochromia,
		Highlights
	}
}