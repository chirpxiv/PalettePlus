using System.Linq;
using System.Numerics;
using System.Reflection;

using ImGuiNET;

using Dalamud.Game.ClientState.Objects.Types;

using ColorEdit.Structs;
using ColorEdit.Extensions;
using ColorEdit.Palettes;
using ColorEdit.Palettes.Attributes;

namespace ColorEdit.Interface.Components {
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

			var fields = typeof(ColorData).GetFields().ToList();
			fields.Sort((a, b) => b.FieldType == typeof(float) ? -1 : 0);
			foreach (var field in fields)
				if (!DrawField(actor, data, field, ref palette))
					break;
		}

		private static unsafe bool DrawField(GameObject actor, ColorData* ptr, FieldInfo field, ref Palette palette) {
			if (ptr == null) return false;

			object data = *ptr;

			var name = field.Name;
			var label = field.Name;
			var val = field.GetValue(data);

			var active = palette.TryGetValue(name, out var _);
			if (ImGui.Checkbox($"##{name}", ref active)) {
				if (active) {
					palette.Add(name, val!);
				} else {
					palette.Remove(name);

					ptr = actor.UpdateColors();
					if (ptr == null) return false;

					data = *ptr;
					palette.Apply(ref data);
					*ptr = (ColorData)data;
				}
			}

			ImGui.SameLine();

			object? newVal = null;
			if (val is Vector4 vec4) {
				var alpha = field.GetCustomAttributes(true).Any(attr => attr is ShowAlpha);
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
				var slider = (Slider?)field.GetCustomAttributes(true).First(attr => attr is Slider);

				var min = slider != null ? slider.Min : 0;
				var max = slider != null ? slider.Max : 1;

				if (ImGui.DragFloat(label, ref flt, 0.01f, min, max))
					newVal = flt;
			} else ImGui.Text($"Error: Unknown type for '{name}'");

			if (newVal != null) {
				palette.SetValue(name, newVal, active);
				field.SetValue(data, newVal);
				*ptr = (ColorData)data;
			}

			return true;
		}
	}
}