using ImGuiNET;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Services;
using PalettePlus.Extensions;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows.Tabs {
	internal class ActorEdit {
		private Palette DefaultPalette = new();
		private Palette Palette = new();

		private ActorList ActorList = new();
		private PaletteList PaletteList = new();

		private ParamContainer ParamContainer = new();

		internal void Draw(bool firstFrame = false) {
			// Actor list

			ActorList.Draw(SelectActor);

			ImGui.SameLine();

			// Color editor
			ImGui.BeginGroup();
			DrawActorEdit(firstFrame);
			ImGui.EndGroup();
		}

		private unsafe void DrawActorEdit(bool firstFrame = false) {
			if (firstFrame)
				SelectActor(ActorList.Selected);

			var actor = ActorList.Selected;
			if (actor == null) return;

			ImGui.Button("Save"); // TODO Implement

			ImGui.SameLine();

			if (ImGui.Button("Reset")) {
				Palette.ShaderParams.Clear();
				actor.UpdateColors();
				PaletteService.GetCharaPalette(actor, out Palette, out DefaultPalette);
			}

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
				obj = PluginServices.ClientState.LocalPlayer;

			ActorList.Selected = obj;

			// Reconstruct Palette for charas previously modified.
			// This is hacky but it works fine for now.

			if (obj != null)
				GetCharaPalette(obj);
		}

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
			ParamContainer = vals;

			foreach (var (key, value) in Palette.ShaderParams) {
				if (value.Equals(DefaultPalette.ShaderParams[key]))
					Palette.ShaderParams.Remove(key);
			}
		}
	}
}