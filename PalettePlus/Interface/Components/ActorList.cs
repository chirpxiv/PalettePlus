using System.Numerics;
using System.Collections.Generic;

using ImGuiNET;

using PalettePlus.Services;

namespace PalettePlus.Interface.Components; 

public class ActorList {
	private readonly ActorService _actors;
	
	private uint? SelectedId;
	private string SearchStr = string.Empty;

	public ActorList(ActorService _actors) {
		this._actors = _actors;
	}
	
	public void Draw(out uint? selectedId) {
		var width = ImGui.GetContentRegionAvail().X / 4.0f;

		// Max name length is 31, but 32 is a nicer number. :)
		ImGui.SetNextItemWidth(width);
		ImGui.InputTextWithHint("##PP_Actor_Search", "Search...", ref this.SearchStr, 32);

		var actorList = BuildActorList();
		if (ImGui.BeginChildFrame(0x_F1, new Vector2(width, -1))) {
			foreach (var (name, id) in actorList) {
				if (this.SearchStr != string.Empty && !name.ToLower().Contains(this.SearchStr.ToLower()))
					continue;

				var isSelected = id == this.SelectedId;
				if (ImGui.Selectable(name, isSelected))
					this.SelectedId = !isSelected ? id : null;
			}

			ImGui.EndChildFrame();
		}

		selectedId = this.SelectedId;
	}
	
	private Dictionary<string, uint> BuildActorList() {
		var actorList = new Dictionary<string, uint>();
		
		foreach (var chara in this._actors.EnumerateHumans()) {
			var name = chara.Name.TextValue;

			var isGPose = this._actors.IsGPoseActor(chara);

			// Handling for duplicate names, including for GPose actors.
			var exists = actorList.TryGetValue(name, out var prevId);
			if (exists) {
				if (isGPose && !this._actors.IsGPoseActor(prevId)) {
					if (this.SelectedId == prevId)
						this.SelectedId = chara.ObjectIndex;
					actorList.Remove(name);
				} else {
					var x = 2;
					while (exists) {
						name = $"{chara.Name.TextValue} #{x}";
						exists = actorList.ContainsKey(name);
						x++;
					}
				}
			}

			if (isGPose) name += " (GPose)";
			
			actorList.Add(name, chara.ObjectIndex);
		}

		return actorList;
	}
}