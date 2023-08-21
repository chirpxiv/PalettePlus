using System.Collections.Generic;

using Dalamud.Interface.Windowing;

using ImGuiNET;

using PalettePlus.Core;
using PalettePlus.Interface.Windows.Tabs;

namespace PalettePlus.Interface.Windows; 

// ReSharper disable once ClassNeverInstantiated.Global
public class MainWindow : Window {
	// Dependencies

	private readonly IServiceContainer _services;
	
	// Window state

    private readonly List<IWindowTab> Tabs = new();
	
    // Initialization
    
	public MainWindow(IServiceContainer _services) : base("Palette+") {
		this._services = _services;
		this.AddTab<ActorTab>();
	}

	private MainWindow AddTab<T>() where T : IWindowTab {
		var inst = this._services.Inject<T>();
		this.Tabs.Add(inst);
		return this;
	}
	
	// Draw current tab

	public override void Draw() {
		if (ImGui.BeginTabBar("PP_Main_Tabs"))
			this.Tabs.ForEach(DrawTabItem);
	}

	private void DrawTabItem(IWindowTab tab) {
		if (ImGui.BeginTabItem(tab.Name))
			tab.Draw();
	}
}