using Dalamud.Plugin;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;

namespace PalettePlus {
	internal class Services {
		[PluginService] internal static DalamudPluginInterface Interface { get; set; } = null!;
		[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
		[PluginService] internal static ObjectTable ObjectTable { get; set; } = null!;
		[PluginService] internal static ClientState ClientState { get; set; } = null!;
		[PluginService] internal static SigScanner SigScanner { get; set; } = null!;

		internal static void Init(DalamudPluginInterface dalamud) => dalamud.Create<Services>();
	}
}