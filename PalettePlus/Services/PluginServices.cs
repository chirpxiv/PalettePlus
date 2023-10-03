using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Plugin.Services;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace PalettePlus.Services {
	internal class PluginServices {
		[PluginService] internal static DalamudPluginInterface Interface { get; set; } = null!;
		[PluginService] internal static ICommandManager CommandManager { get; set; } = null!;
		[PluginService] internal static IGameInteropProvider Hooks { get; set; } = null!;
		[PluginService] internal static IObjectTable ObjectTable { get; set; } = null!;
		[PluginService] internal static IClientState ClientState { get; set; } = null!;
		[PluginService] internal static ISigScanner SigScanner { get; set; } = null!;
		[PluginService] internal static IFramework Framework { get; set; } = null!;
		[PluginService] internal static IPluginLog Log { get; set; } = null!;

		internal unsafe static TargetSystem* Targets = TargetSystem.Instance();

		internal static void Init(DalamudPluginInterface api) => api.Create<PluginServices>();
	}
}
