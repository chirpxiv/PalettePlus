﻿using System.Linq;
using System.Numerics;
using System.Reflection;

using ImGuiNET;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Extensions;
using PalettePlus.Palettes;
using PalettePlus.Palettes.Attributes;

namespace PalettePlus.Interface.Components {
	internal static class PaletteEditor {
		public static void Draw(GameObject actor, ref Palette palette) {
			if (ImGui.BeginChildFrame(2, new Vector2(-1, -1))) {
				DrawColorOptions(actor, ref palette);
				ImGui.EndChildFrame();
			}
		}

		private static unsafe void DrawColorOptions(GameObject actor, ref Palette palette) {
			var model = Model.GetModel(actor);
			if (model == null) return;

			var data = model->GetColorData();
			if (data == null) return;

			ImGui.Spacing();

			var hetchro = (palette.Conditions & PaletteConditions.Heterochromia) != 0;
			if (ImGui.Checkbox("Heterochromia", ref hetchro)) {
				if (hetchro) palette.Remove("RightEyeColor");
				palette.Conditions ^= PaletteConditions.Heterochromia;
			}

			ImGui.SameLine();

			var highlights = (palette.Conditions & PaletteConditions.Highlights) != 0;
			if (ImGui.Checkbox("Highlights", ref highlights)) {
				if (highlights) palette.Remove("HighlightsColor");
				palette.Conditions ^= PaletteConditions.Highlights;
			}

			ImGui.Spacing();
			ImGui.Separator();
			ImGui.Spacing();

			var fields = typeof(ModelParams).GetFields().ToList();
			fields.Sort((a, b) => b.FieldType == typeof(float) ? -1 : 0);
			foreach (var field in fields)
				if (!DrawField(actor, data, field, ref palette))
					break;
		}

		private static unsafe bool DrawField(GameObject actor, ModelParams* ptr, FieldInfo field, ref Palette palette) {
			if (ptr == null) return false;

			bool draw = true;

			object data = *ptr;

			var name = field.Name;
			var label = field.Name;
			var val = field.GetValue(data);
			var attributes = field.GetCustomAttributes();

			var active = palette.ContainsKey(name);
			var _active = active;

			object? newVal = null;

			var condition = (Conditional?)attributes.FirstOrDefault(attr => attr is Conditional);
			if (condition != null) {
				draw = (palette.Conditions & condition.Conditions) != 0;
				if (!draw) {
					if (active) active = false;
					if (condition is ConditionalLink linked) {
						var linkVal = typeof(ModelParams).GetField(linked.LinkedTo)!.GetValue(data);
						if (val != linkVal) newVal = linkVal;
					}
				}
			}

			if (draw) {
				ImGui.Checkbox($"##{name}", ref active);

				ImGui.SameLine();

				if (val is Vector4 vec4) {
					var alpha = attributes.Any(attr => attr is ShowAlpha);
					if (alpha) {
						if (ImGui.ColorEdit4(label, ref vec4))
							newVal = vec4;
					} else {
						var vec3 = new Vector3(vec4.X, vec4.Y, vec4.Z);
						if (ImGui.ColorEdit3(label, ref vec3))
							newVal = new Vector4(vec3, vec4.W);
					}
				} else if (val is Vector3 vec3) {
					if (ImGui.ColorEdit3(label, ref vec3))
						newVal = vec3;
				} else if (val is float flt) {
					var slider = (Slider?)attributes.First(attr => attr is Slider);

					var min = slider != null ? slider.Min : 0;
					var max = slider != null ? slider.Max : 1;

					if (ImGui.DragFloat(label, ref flt, 0.01f, min, max))
						newVal = flt;
				} else ImGui.Text($"Error: Unknown type for '{name}'");
			}

			if (active != _active) {
				if (active) {
					palette.Add(name, val!);
				} else {
					palette.Remove(name);

					ptr = actor.UpdateColors();
					if (ptr == null) return false;

					data = *ptr;
					palette.Apply(ref data);
					*ptr = (ModelParams)data;
				}
			}

			if (newVal != null) {
				palette.SetValue(name, newVal, active);
				field.SetValue(data, newVal);
				*ptr = (ModelParams)data;
			}

			return true;
		}
	}
}