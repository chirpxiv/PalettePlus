using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Extensions;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows {
	public class MainWindow : Window {
		private const int GPoseStartIndex = 201;

		private GameObject? Selected = null;

		private Palette DefaultPalette = new();
		private Palette Palette = new();

		private ActorList ActorList = new();

		// Window

		public MainWindow() : base(
			"Palette+"
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

		private unsafe void DrawActorEdit() {
			var actor = Selected;
			if (actor == null) return;

			ImGui.Button("Save"); // TODO Implement

			ImGui.SameLine();

			//ImGui.BeginDisabled(Palette.Count == 0);
			if (ImGui.Button("Reset")) {
				Palette.ShaderParams.Clear();
				actor.UpdateColors();
			}
			//ImGui.EndDisabled();

			var model = Model.GetModel(actor);
			if (model != null) {
				var cont = new ParamContainer {
					Model = *model->ModelShader->ModelParams,
					Decal = *model->DecalShader->DecalParams
				};

				if (PaletteEditor.Draw(DefaultPalette, ref Palette, ref cont)) {
					*model->ModelShader->ModelParams = cont.Model;
					*model->DecalShader->DecalParams = cont.Decal;
				}
			}
		}

		private unsafe void SelectActor(GameObject? obj) {
			if (obj == null)
				obj = Services.ClientState.LocalPlayer;

			Selected = obj;
			Palette.ShaderParams.Clear();

			// Reconstruct Palette for charas previously modified.
			// This is hacky but it works fine for now.

			var model = obj != null ? Model.GetModel(obj) : null;
			var color = model != null ? model->GetColorData() : null;
			if (color != null) {
				Palette.CopyShaderParams(*color);

				DefaultPalette = new Palette();
				DefaultPalette.CopyShaderParams(model->GenerateColorValues().Model);

				foreach (var (key, value) in Palette.ShaderParams) {
					if (value.Equals(DefaultPalette.ShaderParams[key]))
						Palette.ShaderParams.Remove(key);
				}
			}
		}

		// "Saved Palettes" Tab

		private void DrawPalettesTab() {
			// TODO
		}
	}
}