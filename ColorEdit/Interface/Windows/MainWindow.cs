using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState.Objects.Types;

using ColorEdit.Structs;
using ColorEdit.Extensions;
using ColorEdit.Palettes;
using ColorEdit.Palettes.Attributes;
using ColorEdit.Interface.Components;

namespace ColorEdit.Interface.Windows {
	public class MainWindow : Window {
		private const int GPoseStartIndex = 201;

		private GameObject? Selected = null;
		private Palette Palette = new();

		private ActorList ActorList = new();

		// Window

		public MainWindow() : base(
			"ColorEdit"
		) {
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

		// Selection

		private unsafe void Select(GameObject? obj) {
			if (obj == null)
				obj = Services.ClientState.LocalPlayer;

			Selected = obj;
			Palette.Clear();

			// Reconstruct Palette for charas previously modified.
			// This is hacky but it works fine for now.

			var model = obj != null ? Model.GetModel(obj) : null;
			var color = model != null ? model->GetColorData() : null;
			if (color != null) {
				Palette.Copy(*color);

				var basic = new Palette();
				color = obj!.UpdateColors();
				if (color == null) return;

				basic.Copy(*color);

				foreach (var (key, value) in Palette) {
					if (Palette[key].Equals(basic[key]))
						Palette.Remove(key);
				}

				object data = *color;
				Palette.Apply(ref data);
				*color = (DrawParams)data;
			}
		}

		// Tabs

		private void DrawTab(string label, Action callback) {
			if (ImGui.BeginTabItem(label)) {
				callback.Invoke();
				ImGui.EndTabItem();
			}
		}

		// "Edit Players" Tab

		private void EditPlayersTab() {
			var size = ImGui.GetWindowSize();

			// Actor list
			
			ActorList.Draw(Selected, Select);

			ImGui.SameLine();

			// Color editor
			ImGui.BeginGroup();
			DrawActorEdit();
			ImGui.EndGroup();
		}

		// Actor edit

		private void DrawActorEdit() {
			var actor = Selected;
			if (actor == null) return;

			ImGui.Button("Save"); // TODO Implement

			ImGui.SameLine();

			ImGui.BeginDisabled(Palette.Count == 0);
			if (ImGui.Button("Reset")) {
				Palette.Clear();
				unsafe { actor.UpdateColors(); }
			}
			ImGui.EndDisabled();

			ImGui.SameLine();

			var eyeLink = (ColorEdit.Config.Linked & LinkType.Eyes) != LinkType.None;
			if (ImGui.Checkbox("Link eye colors", ref eyeLink))
				ColorEdit.Config.Linked ^= LinkType.Eyes;

			PaletteEditor.Draw(actor, ref Palette);
		}
	}
}