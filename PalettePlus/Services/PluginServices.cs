using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace PalettePlus.Services {
	internal class PluginServices {
		[PluginService] internal static DalamudPluginInterface Interface { get; set; } = null!;
		[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
		[PluginService] internal static ObjectTable ObjectTable { get; set; } = null!;
		[PluginService] internal static ClientState ClientState { get; set; } = null!;
		[PluginService] internal static SigScanner SigScanner { get; set; } = null!;
		[PluginService] internal static Framework Framework { get; set; } = null!;

		internal unsafe static TargetSystem* Targets = TargetSystem.Instance();

		internal static void Init(DalamudPluginInterface dalamud) => dalamud.Create<PluginServices>();
	}
}