using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;

using ColorEdit.Structs;

namespace ColorEdit.Interface.Windows {
	public class MainWindow : Window {
		public MainWindow() : base(
			"ColorEdit"
		) {
			RespectCloseHotkey = false;
		}

		public unsafe override void Draw() {
			var player = ColorEdit.GetPlayer();
			if (player == null) return;

			var model = Model.GetModelFor( Services.ObjectTable.CreateObjectReference((IntPtr)Services.Targets->GPoseTarget) ?? player );
			if (model == null) return;

			DebugText(string.Format(
				"ColorData* {0:X}",
				(IntPtr)model->ModelData
			));
			DebugText(string.Format(
				"Data {0:X}",
				(IntPtr)model->ModelData->ColorData
			));

			var col = model->ModelData->ColorData->SkinTone;
			ImGui.ColorEdit3("SkinTone", ref col, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->SkinTone = col;

			var lipCol = model->ModelData->ColorData->LipColor;
			ImGui.ColorEdit4("LipColor", ref lipCol, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->LipColor = lipCol;

			var hairCol = model->ModelData->ColorData->HairColor;
			ImGui.ColorEdit4("HairColor", ref hairCol, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->HairColor = hairCol;

			var hairShine = model->ModelData->ColorData->HairShine;
			ImGui.ColorEdit4("HairShine", ref hairShine, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->HairShine = hairShine;

			var hlCol = model->ModelData->ColorData->HighlightsColor;
			ImGui.ColorEdit4("HighlightsColor", ref hlCol, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->HighlightsColor = hlCol;

			var leftEye = model->ModelData->ColorData->LeftEye;
			ImGui.ColorEdit4("LeftEye", ref leftEye, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->LeftEye = leftEye;

			ImGui.SameLine();

			var rightEye = model->ModelData->ColorData->RightEye;
			ImGui.ColorEdit4("RightEye", ref rightEye, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->RightEye = rightEye;

			var limbal = model->ModelData->ColorData->LimbalRingColor;
			ImGui.ColorEdit4("LimbalRingColor", ref limbal, ImGuiColorEditFlags.NoInputs);
			model->ModelData->ColorData->LimbalRingColor = limbal;
		}

		private void DebugText(string text) {
			ImGui.Text(text);
			if (ImGui.IsItemClicked())
				ImGui.SetClipboardText(text);
		}
	}
}