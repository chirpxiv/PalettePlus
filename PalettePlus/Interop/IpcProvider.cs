using System;

using Dalamud.Plugin.Ipc;
using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Services;
using PalettePlus.Palettes;
using Dalamud.Logging;

namespace PalettePlus.Interop {
	public static class IpcProvider {
		public const string API_VERSION = "1.0.0";

		public const string ApiVersionStr = "PalettePlus.ApiVersion";
		public const string GetCharaPaletteStr = "PalettePlus.GetCharaPalette";
		public const string SetCharaPaletteStr = "PalettePlus.SetCharaPalette";
		public const string RemoveCharaPaletteStr = "PalettePlus.RemoveCharaPalette";
		public const string BuildCharaPaletteStr = "PalettePlus.BuildCharaPalette";

		private static ICallGateProvider<string>? IApiVersion;

		private static ICallGateProvider<Character, string>? IGetCharaPalette;
		private static ICallGateProvider<Character, string, object>? ISetCharaPalette;
		private static ICallGateProvider<Character, object>? IRemoveCharaPalette;

		// Constructs a new Palette object from character memory instead of fetching from cache.
		private static ICallGateProvider<Character, string>? IBuildCharaPalette;

		internal static void Init() {
			try {
				IApiVersion = PluginServices.Interface.GetIpcProvider<string>(ApiVersionStr);
				IApiVersion.RegisterFunc(ApiVersion);

				IGetCharaPalette = PluginServices.Interface.GetIpcProvider<Character, string>(GetCharaPaletteStr);
				IGetCharaPalette.RegisterFunc(GetCharaPalette);

				ISetCharaPalette = PluginServices.Interface.GetIpcProvider<Character, string, object>(SetCharaPaletteStr);
				ISetCharaPalette.RegisterAction(SetCharaPalette);

				IRemoveCharaPalette = PluginServices.Interface.GetIpcProvider<Character, object>(RemoveCharaPaletteStr);
				IRemoveCharaPalette.RegisterAction(RemoveCharaPalette);

				IBuildCharaPalette = PluginServices.Interface.GetIpcProvider<Character, string>(BuildCharaPaletteStr);
				IBuildCharaPalette.RegisterFunc(BuildCharaPalette);
			} catch (Exception e) {
				PluginLog.Error("Failed to initialise Palette+ IPC", e);
			}
		}

		internal static void Dispose() {
			IApiVersion?.UnregisterFunc();
			IGetCharaPalette?.UnregisterFunc();
			ISetCharaPalette?.UnregisterAction();
			IRemoveCharaPalette?.UnregisterAction();
			IBuildCharaPalette?.UnregisterFunc();
		}

		// IPC Methods

		private static string ApiVersion() => API_VERSION;

		private static string GetCharaPalette(Character chara)
			=> PaletteService.GetCharaPalette(chara).ToString();

		private static void SetCharaPalette(Character chara, string json)
			=> PaletteService.SetCharaPalette(chara, Palette.FromJson(json));

		private static void RemoveCharaPalette(Character chara)
			=> PaletteService.RemoveCharaPalette(chara);

		private static string BuildCharaPalette(Character chara) {
			PaletteService.BuildCharaPalette(chara, out var palette, out _, false);
			return palette.ToString();
		}

	}
}