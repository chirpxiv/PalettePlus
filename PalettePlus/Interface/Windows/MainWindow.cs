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

		private Palette DefaultPalette = new();
		private Palette Palette = new();

		private ActorList ActorList = new();
		private PaletteList PaletteList = new();

		private ParamContainer ParamContainer = new();

		// Window

		public MainWindow() : base(
			"Palette+"
		) {
			SizeConstraints = new WindowSizeConstraints {
				MinimumSize = new Vector2(470, 210),
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

		private string CurrentTab = "";
		private bool TabSwitched = false;

		private void DrawTab(string label, Action callback) {
			if (ImGui.BeginTabItem(label)) {
				var switched = CurrentTab != label;
				if (switched) {
					CurrentTab = label;
					TabSwitched = true;
				}

				callback.Invoke();
				ImGui.EndTabItem();

				if (switched)
					TabSwitched = false;
			}
		}

		// "Edit Players" Tab

		private void DrawPlayersTab() {
			// Actor list
			
			ActorList.Draw(SelectActor);

			ImGui.SameLine();

			// Color editor
			ImGui.BeginGroup();
			DrawActorEdit();
			ImGui.EndGroup();
		}

		private unsafe void DrawActorEdit() {
			var actor = ActorList.Selected;
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
				ParamContainer.Model = *model->ModelShader->ModelParams;
				ParamContainer.Decal = *model->DecalShader->DecalParams;

				if (PaletteEditor.Draw(DefaultPalette, ref Palette, ref ParamContainer)) {
					*model->ModelShader->ModelParams = ParamContainer.Model;
					*model->DecalShader->DecalParams = ParamContainer.Decal;
				}
			}
		}

		private unsafe void SelectActor(GameObject? obj) {
			if (obj == null)
				obj = Services.ClientState.LocalPlayer;

			ActorList.Selected = obj;

			// Reconstruct Palette for charas previously modified.
			// This is hacky but it works fine for now.

			if (obj != null)
				GetCharaPalette(obj);
		}

		// "Saved Palettes" Tab

		private unsafe void DrawPalettesTab() {
			var select = PaletteList.Draw();
			if (select || TabSwitched) {
				var local = Services.ClientState.LocalPlayer;
				if (local != null)
					GetCharaPalette(local, true);

				if (PaletteList.Selected != null) {
					var model = (object)ParamContainer.Model;
					PaletteList.Selected.ApplyShaderParams(ref model);
					ParamContainer.Model = (ModelParams)model;

					var decal = (object)ParamContainer.Decal;
					PaletteList.Selected.ApplyShaderParams(ref decal);
					ParamContainer.Decal = (DecalParams)decal;
				}
			}

			ImGui.SameLine();

			ImGui.BeginGroup();

			if (PaletteList.Selected != null)
				PaletteEditor.Draw(DefaultPalette, ref PaletteList.Selected, ref ParamContainer, true);

			ImGui.EndGroup();
		}

		// Util

		private unsafe void GetCharaPalette(GameObject obj, bool contain = false) {
			var model = Model.GetModel(obj);
			var color = model != null ? model->GetModelParams() : null;
			if (color == null) return;

			// TODO: Refactor

			Palette.ShaderParams.Clear();

			Palette.CopyShaderParams(*color);

			var decal = model->GetDecalParams();
			if (decal != null)
				Palette.CopyShaderParams(*decal);

			DefaultPalette = new Palette();

			var vals = model->GenerateColorValues();
			DefaultPalette.CopyShaderParams(vals.Model);
			DefaultPalette.CopyShaderParams(vals.Decal);
			if (contain) ParamContainer = vals;

			foreach (var (key, value) in Palette.ShaderParams) {
				if (value.Equals(DefaultPalette.ShaderParams[key]))
					Palette.ShaderParams.Remove(key);
			}
		}
	}
}