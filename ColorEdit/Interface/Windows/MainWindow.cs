using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState.Objects.Types;

using ColorEdit.Structs;
using ColorEdit.Palettes;
using ColorEdit.Extensions;
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
				DrawTab("Edit Players", DrawPlayersTab);
				DrawTab("Saved Palettes", DrawPalettesTab);
				DrawTab("Persistence", () => {});
				ImGui.EndTabBar();
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

		private void DrawPlayersTab() {
			var size = ImGui.GetWindowSize();

			// Actor list
			
			ActorList.Draw(Selected, SelectActor);

			ImGui.SameLine();

			// Color editor
			ImGui.BeginGroup();
			DrawActorEdit();
			ImGui.EndGroup();
		}

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

			PaletteEditor.Draw(actor, ref Palette);
		}

		private unsafe void SelectActor(GameObject? obj) {
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

				var def = new Palette();
				def.Copy(model->GenerateColorValues().Model);

				foreach (var (key, value) in Palette) {
					if (Palette[key].Equals(def[key]))
						Palette.Remove(key);
				}
			}
		}

		// "Saved Palettes" Tab

		private void DrawPalettesTab() {
			// TODO
		}
	}
}