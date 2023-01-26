using ImGuiNET;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Services;
using PalettePlus.Extensions;
using PalettePlus.Interface.Dialog;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows.Tabs {
	internal class ActorEdit {
		private Palette DefaultPalette = new();
		private Palette Palette = new();

		internal ActorList ActorList = new();
		private PaletteList PaletteList = new();

		private ParamContainer ParamContainer = new();

		internal void Draw(bool firstFrame) {
			// Actor list

			ActorList.Draw(SelectActor);

			ImGui.SameLine();

			// Color editor
			ImGui.BeginGroup();
			DrawActorEdit(firstFrame);
			ImGui.EndGroup();
		}

		private unsafe void DrawActorEdit(bool firstFrame) {
			if (firstFrame)
				SelectActor(ActorList.Selected);

			var actor = ActorList.Selected;
			if (actor == null) return;

			if (ImGui.Button("Save")) {
				var name = "";
				string? err = null;

				var popup = (PopupMenu)PluginGui.GetWindow<PopupMenu>();
				popup.Open(() => {
					if (NameInput.Draw(ref name, ref err)) {
						popup.Close();

						var palette = (Palette)Palette.Clone();
						palette.Name = name;
						PalettePlus.Config.SavedPalettes.Add(palette);
					}
				});
			}

			ImGui.SameLine();

			if (ImGui.Button("Reset")) {
				PaletteService.RemoveCharaPalette(actor);

				Palette.ShaderParams.Clear();

				actor.UpdateColors();
				PaletteService.BuildCharaPalette(actor, out Palette, out DefaultPalette);
			}

			var model = Model.GetModel(actor);
			if (model != null) {
				ParamContainer.Model = *model->ModelShader->ModelParams;
				ParamContainer.Decal = *model->DecalShader->DecalParams;

				if (PaletteEditor.Draw(DefaultPalette, ref Palette, ref ParamContainer)) {
					*model->ModelShader->ModelParams = ParamContainer.Model;
					*model->DecalShader->DecalParams = ParamContainer.Decal;

					PaletteService.SetCharaPalette(actor, (Palette)Palette.Clone());
				}
			}
		}

		private unsafe void SelectActor(Character? obj) {
			if (obj == null)
				obj = PluginServices.ClientState.LocalPlayer;

			if (obj != null && !obj.IsValidForPalette())
				obj = null;

			ActorList.Selected = obj;

			if (obj != null)
				PaletteService.BuildCharaPalette(obj, out Palette, out DefaultPalette, true);
		}
	}
}