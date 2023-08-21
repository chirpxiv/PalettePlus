using Dalamud.Plugin;

using PalettePlus.Core;
using PalettePlus.Interop;
using PalettePlus.Interface;
using PalettePlus.Services;

namespace PalettePlus;

// ReSharper disable once UnusedType.Global
public sealed class PalettePlus : IDalamudPlugin {
	public string Name => "Palette+ (v0.4)";

	private readonly ServiceManager Services;

	public PalettePlus(DalamudPluginInterface api) {
		// Register services
		this.Services = new ServiceManager()
			.AddDalamudServices(api)
			.AddService<InteropService>()
			.AddService<PaletteService>()
			.AddService<GuiService>()
			.AddService<CommandService>();
		
		// and initialize them
		this.Services.GetRequiredService<GuiService>();
		this.Services.GetRequiredService<CommandService>();
		this.Services.GetRequiredService<PaletteService>();
	}

	public void Dispose() {
		this.Services.Dispose();
	}
}