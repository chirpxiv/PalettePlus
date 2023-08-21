using ImGuiNET;

using PalettePlus.Services;
using PalettePlus.Interface.Components;

namespace PalettePlus.Interface.Windows.Tabs; 

// ReSharper disable once ClassNeverInstantiated.Global
public class ActorTab : IWindowTab {
	// Dependencies & components

	private readonly ActorService _actors;

	private readonly ActorList ActorList;
	
	// Tab state

	private uint? SelectedId;
	
	// Tab draw
	
	public string Name { get; init; } = "Actors";

	public ActorTab(ActorService _actors) {
		this._actors = _actors;
		this.ActorList = new ActorList(_actors);
	}

	public void Draw() {
		ImGui.BeginGroup();
		this.ActorList.Draw(out this.SelectedId);
		ImGui.EndGroup();
	}
}