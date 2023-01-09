using Dalamud.Plugin;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;

using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace PalettePlus {
	internal class Services {
		[PluginService] internal static DalamudPluginInterface Interface { get; set; } = null!;
		[PluginService] internal static CommandManager CommandManager { get; set; } = null!;
		[PluginService] internal static ObjectTable ObjectTable { get; set; } = null!;
		[PluginService] internal static ClientState ClientState { get; set; } = null!;
		[PluginService] internal static SigScanner SigScanner { get; set; } = null!;
		[PluginService] internal static GameGui GameGui { get; set; } = null!;

		internal unsafe static TargetSystem* Targets = TargetSystem.Instance();

		internal static void Init(DalamudPluginInterface dalamud) => dalamud.Create<Services>();
	}
}