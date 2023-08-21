using ImGuiNET;

using PalettePlus.Services;

namespace PalettePlus.Interface.Windows.Tabs; 

// ReSharper disable once ClassNeverInstantiated.Global
public class ActorTab : IWindowTab {
	// Dependencies

	private readonly ActorService _actors;
	
	// Window
	
	public string Name { get; init; } = "Actors";

	public ActorTab(ActorService _actors) {
		this._actors = _actors;
	}

	public void Draw() {
		foreach (var actor in this._actors.EnumerateHumans()) {
			ImGui.Text($"{actor.Name.ToString()} ({actor.ObjectIndex})");
		}
	}
}