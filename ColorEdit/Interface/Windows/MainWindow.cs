using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Collections.Generic;

using ImGuiNET;

using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState.Objects.Types;

using ColorEdit.Structs;
using ColorEdit.Interop;
using ColorEdit.Palettes;
using ColorEdit.Palettes.Attributes;

namespace ColorEdit.Interface.Windows {
	public class MainWindow : Window {
		private const int GPoseStartIndex = 201;

		private static Dictionary<string, string> PropertyLabels = new() {};

		private string SearchString = "";

		private GameObject? Selected = null;
		private Palette Palette = new();

		private Dictionary<string, ActorContainer> ActorNames = new();

		// Window

		public MainWindow() : base(
			"ColorEdit"
		) {
			SelectSelf();
			RespectCloseHotkey = false;
			SizeConstraints = new WindowSizeConstraints {
				MinimumSize = new Vector2(400, 200),
				MaximumSize = ImGui.GetIO().DisplaySize
			};
		}

		public override void Draw() {
			if (ImGui.BeginTabBar("ColorEdit Tabs")) {
				DrawTab("Edit Players", EditPlayersTab);
				ImGui.EndTabBar();
			}
		}

		public override void OnClose() {
			base.OnClose();
			ActorNames.Clear();
		}

		// Selection

		private void Select(GameObject? obj) {
			Selected = obj;
			Palette.Clear();
		}

		private void SelectSelf() => Select(Services.ClientState.LocalPlayer);

		// Tabs

		private void DrawTab(string label, Action callback) {
			if (ImGui.BeginTabItem(label)) {
				callback.Invoke();
				ImGui.EndTabItem();
			}
		}

		private void EditPlayersTab() {
			var size = ImGui.GetWindowSize();

			// Actor list
			ImGui.BeginGroup();

			var leftWidth = Math.Min(size.X * 1 / 3, 400);

			ImGui.SetNextItemWidth(leftWidth);
			if (ImGui.InputTextWithHint("##ActorSearch", "Search...", ref SearchString, 64)) {
				// TODO Implement
			}

			if (ImGui.BeginChildFrame(1, new Vector2(leftWidth, -1))) {
				DrawActorList();
				ImGui.EndChildFrame();
			}
			ImGui.EndGroup();

			ImGui.SameLine();

			// Color editor
			ImGui.BeginGroup();
			DrawActorEdit();
			ImGui.EndGroup();
		}

		// Actor list

		private void DrawActorList() {
			ActorNames.Clear();

			bool isSelectionValid = false;
			for (var i = 0; i < GPoseStartIndex + 200; i++) {
				var actor = Services.ObjectTable[i];
				if (actor == null) continue;

				var name = actor.Name.ToString();
				if (name.Length == 0) continue;

				if (Selected == actor)
					isSelectionValid = true;

				var cont = new ActorContainer(i, actor);

				var exists = ActorNames.TryGetValue(name, out var _);
				if (exists) {
					if (i >= GPoseStartIndex) {
						if (ActorNames[name].GameObject == Selected)
							Select(actor);
						ActorNames[name] = cont;
					} else {
						var x = 2;
						while (exists) {
							name = $"{actor.Name} #{x}";
							exists = ActorNames.TryGetValue(name, out var _);
							x++;
						}
						ActorNames.Add(name, cont);
					}
				} else {
					ActorNames.Add(name, cont);
				}
			}

			if (!isSelectionValid)
				SelectSelf();

			foreach (var (name, cont) in ActorNames) {
				var actor = cont.GameObject;

				var label = name;
				if (cont.IsGPoseActor)
					label += " (GPose)";

				if (ImGui.Selectable(label, Selected == actor))
					Select(actor);
			}
		}

		// Actor edit

		private void DrawActorEdit() {
			var actor = Selected;
			if (actor == null) return;

			ImGui.Button("Save"); // TODO Implement
			ImGui.SameLine();
			if (ImGui.Button("Reset")) {
				Palette.Clear();
				unsafe { UpdateSelected(); }
			}

			if (ImGui.BeginChildFrame(2, new Vector2(-1, -1))) {
				DrawColorOptions(actor);
				ImGui.EndChildFrame();
			}
		}

		private unsafe void DrawColorOptions(GameObject actor) {
			var model = Model.GetModelFor(actor);
			if (model == null) return;

			var data = model->GetColorData();
			if (data == null) return;

			var fields = typeof(ColorData).GetFields().ToList();
			fields.Sort((a, b) => b.FieldType == typeof(float) ? -1 : 0);
			foreach (var field in fields)
				DrawField(data, field);
		}

		private unsafe void DrawField(ColorData* ptr, FieldInfo field) {
			object data = *ptr;

			var name = field.Name;
			var label = field.Name;
			var val = field.GetValue(data);

			var active = Palette.TryGetValue(name, out var _);
			if (ImGui.Checkbox($"##{name}", ref active)) {
				if (active) {
					Palette.Add(name, val!);
				} else {
					Palette.Remove(name);

					ptr = UpdateSelected();
					if (ptr == null) return;

					data = *ptr;
					Palette.Apply(ref data);
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
				Palette.SetValue(name, newVal, active);
				field.SetValue(data, newVal);
				*ptr = (ColorData)data;
			}
		}

		private unsafe ColorData* UpdateSelected() {
			if (Selected == null) return null;

			var model = Model.GetModelFor(Selected);
			if (model == null) return null;

			Hooks.UpdateColorsHook.Original(model);

			return model->GetColorData();
		}

		// ActorContainer

		private class ActorContainer {
			internal int Index;
			internal GameObject GameObject;

			internal ActorContainer(int index, GameObject obj) {
				Index = index;
				GameObject = obj;
			}

			internal bool IsGPoseActor => Index >= GPoseStartIndex;
		}
	}
}