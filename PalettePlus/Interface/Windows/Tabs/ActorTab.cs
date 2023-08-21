using ImGuiNET;

namespace PalettePlus.Interface.Windows.Tabs; 

// ReSharper disable once ClassNeverInstantiated.Global
public class ActorTab : IWindowTab {
	public string Name { get; init; } = "Actors";

	public void Draw() {
		ImGui.Text("Hi :)");
	}
}