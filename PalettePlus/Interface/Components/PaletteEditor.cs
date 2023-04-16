using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Collections.Generic;

using Dalamud.Logging;

using ImGuiNET;

using PalettePlus.Extensions;
using PalettePlus.Services;
using PalettePlus.Palettes;
using PalettePlus.Palettes.Attributes;

namespace PalettePlus.Interface.Components {
	public static class PaletteEditor {
		private static Array ConditionEnums = Enum.GetValues(typeof(PaletteConditions));

		private static Dictionary<Type, FieldInfo> ContainerFields = new();
		private static List<PaletteField> PaletteFields = new();

		static PaletteEditor() {
			// build reflection cache

			var paramFields = typeof(ParamContainer).GetFields();
			foreach (var paramField in paramFields) {
				var type = paramField.FieldType;
				ContainerFields.Add(type, paramField);

				var fields = type.GetFields();
				foreach (var field in fields) {
					var value = new PaletteField(field);

					var link = (ConditionalLink?)value.Attributes.FirstOrDefault(attr => attr is ConditionalLink);
					if (link != null) link.LinkedField = fields.First(f => f.Name == link.LinkedTo);

					PaletteFields.Add(value);
				}
			}

			PaletteFields.Sort((a,b) => b.FieldInfo.FieldType == typeof(float) ? -1 : 0);
		}

		public static bool Draw(Palette defaults, ref Palette palette, ref ParamContainer cont) {
			var result = false;
			if (ImGui.BeginChildFrame(2, new Vector2(-1, -1))) {
				DrawToggles(ref palette);
				result = DrawFields(defaults, ref palette, ref cont);
				ImGui.EndChildFrame();
			}
			return result;
		}

		public static void DrawToggles(ref Palette palette) {
			ImGui.Spacing();

			foreach (var condObj in ConditionEnums) {
				var cond = (PaletteConditions)condObj;
				if (cond == 0) continue;

				if ((int)cond > 1) ImGui.SameLine();

				var val = (palette.Conditions & cond) != 0;
				if (ImGui.Checkbox(cond.ToString(), ref val))
					palette.Conditions ^= cond;
			}

			ImGui.Spacing();
		}

		public static bool DrawFields(Palette defaults, ref Palette palette, ref ParamContainer cont) {
			var result = false;
			foreach (var field in PaletteFields)
				result |= DrawField(defaults, ref palette, ref cont, field);
			return result;
		}

		private static bool DrawField(Palette defaults, ref Palette palette, ref ParamContainer cont, PaletteField field) {
			var contParam = ContainerFields[field.ReflectedType];
			object contBox = cont;

			var param = contParam.GetValue(contBox);
			if (param == null) return false;

			var data = field.FieldInfo.GetValue(param);
			if (data == null) return false;

			var draw = true;

			var key = field.FieldInfo.Name;
			var label = PaletteService.GetLabel(key);
			if (key == "LeftEyeColor" && (palette.Conditions & PaletteConditions.Heterochromia) == 0)
				label = "Eye Color";

			var active = palette.ShaderParams.ContainsKey(key);
			var _active = active;

			object? newVal = null;

			var condition = (Conditional?)field.Attributes.FirstOrDefault(attr => attr is Conditional);
			if (condition != null) {
				var match = (palette.Conditions & condition.Conditions) != 0;
				draw &= match;

				if (condition is ConditionalLink linked) {
					var linkVal = linked.LinkedField!.GetValue(param);
					if (!match && !data.Equals(linkVal))
						newVal = linkVal;

					if (linked.LastResult != match) {
						linked.LastResult = match;
						if (match)
							active = palette.ShaderParams.ContainsKey(linked.LinkedTo);
						else
							active = false;
					}
				}
			}

			if (draw) {
				ImGui.Checkbox($"##{field.FieldInfo.Name}", ref active);

				ImGui.SameLine();

				if (data is Vector4 vec4) {
					vec4 = vec4.RgbSqrt();
					
					var alpha = field.Attributes.Any(attr => attr is ShowAlpha);
					if (alpha) {
						if (ImGui.ColorEdit4(label, ref vec4))
							newVal = vec4.RgbPow2();
					} else {
						var vec3 = new Vector3(vec4.X, vec4.Y, vec4.Z);
						if (ImGui.ColorEdit3(label, ref vec3))
							newVal = new Vector4(vec3.RgbPow2(), vec4.W);
					}
				} else if (data is Vector3 vec3) {
					vec3 = vec3.RgbSqrt();
					if (ImGui.ColorEdit3(label, ref vec3))
						newVal = vec3.RgbPow2();
				} else if (data is float flt) {
					var slider = (Slider?)field.Attributes.FirstOrDefault(attr => attr is Slider);

					var min = slider != null ? slider.Min : 0;
					var max = slider != null ? slider.Max : 1;

					if (ImGui.DragFloat(label, ref flt, 0.01f, min, max))
						newVal = flt;
				} else {
					ImGui.Text($"[Not Implemented] {field.FieldInfo.Name} {data}");
				}
			}

			active |= newVal != null;

			if (active != _active) {
				if (active) {
					palette.ShaderParams.Add(key, data);
				} else {
					palette.ShaderParams.Remove(key);
					newVal = defaults.ShaderParams.GetValueOrDefault(field.FieldInfo.Name);
				}
			}

			if (newVal != null) {
				if (active) palette.ShaderParams[key] = newVal;
				field.FieldInfo.SetValue(param, newVal);
				contParam.SetValue(contBox, param);
				return true;
			}

			return false;
		}

		private class PaletteField {
			internal FieldInfo FieldInfo;
			internal Type ReflectedType;
			internal IEnumerable<Attribute> Attributes;

			internal PaletteField(FieldInfo field) { 
				FieldInfo = field;
				ReflectedType = field.ReflectedType!;
				Attributes = field.GetCustomAttributes();
			}
		}
	}
}