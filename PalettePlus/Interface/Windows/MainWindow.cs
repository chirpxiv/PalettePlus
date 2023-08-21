using Dalamud.Interface.Windowing;

using ImGuiNET;

namespace PalettePlus.Interface.Windows; 

public class MainWindow : Window {
	public MainWindow() : base("Palette+") {}

	public override void Draw() {
		ImGui.Text("Hello!");
	}
}