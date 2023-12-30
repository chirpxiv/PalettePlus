using System;

using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Services;
using PalettePlus.Palettes;
using System.Linq;
using Newtonsoft.Json;
using PalettePlus.Extensions;

namespace PalettePlus.Interop {
	public static class IpcProvider {
		public const string API_VERSION = "1.1.0";

		public const string ApiVersionStr = "PalettePlus.ApiVersion";
		public const string GetCharaPaletteStr = "PalettePlus.GetCharaPalette";
		public const string SetCharaPaletteStr = "PalettePlus.SetCharaPalette";
		public const string RemoveCharaPaletteStr = "PalettePlus.RemoveCharaPalette";
		public const string RedrawCharaStr = "PalettePlus.RedrawChara";
		public const string BuildCharaPaletteStr = "PalettePlus.BuildCharaPalette";
		public const string BuildCharaPaletteOrEmptyStr = "PalettePlus.BuildCharaPaletteOrEmpty";
		public const string PaletteChangedStr = "PalettePlus.PaletteChanged";
		public const string GetSavedPalettesStr = "PalettePlus.GetSavedPalettes";

		private static ICallGateProvider<string>? IApiVersion;

		private static ICallGateProvider<Character, string>? IGetCharaPalette;
		private static ICallGateProvider<Character, string, object>? ISetCharaPalette;
		private static ICallGateProvider<Character, object>? IRemoveCharaPalette;
		private static ICallGateProvider<Character, object>? IRedrawChara;
		private static ICallGateProvider<Character, string, object>? IPaletteChanged;

		// Constructs a new Palette object from character memory instead of fetching from cache.
		private static ICallGateProvider<Character, string>? IBuildCharaPalette;
		private static ICallGateProvider<Character, string>? IBuildCharaPaletteOrEmpty;
				
		private static ICallGateProvider<string[]>? IGetSavedPalettes;

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

				IRedrawChara = PluginServices.Interface.GetIpcProvider<Character, object>(RedrawCharaStr);
				IRedrawChara.RegisterAction(RedrawChara);

				IBuildCharaPalette = PluginServices.Interface.GetIpcProvider<Character, string>(BuildCharaPaletteStr);
				IBuildCharaPalette.RegisterFunc(BuildCharaPalette);

				IBuildCharaPaletteOrEmpty = PluginServices.Interface.GetIpcProvider<Character, string>(BuildCharaPaletteOrEmptyStr);
				IBuildCharaPaletteOrEmpty.RegisterFunc(BuildCharaPaletteOrEmpty);

				IPaletteChanged = PluginServices.Interface.GetIpcProvider<Character, string, object>(PaletteChangedStr);
								
				IGetSavedPalettes = PluginServices.Interface.GetIpcProvider<string[]>(GetSavedPalettesStr);
        IGetSavedPalettes.RegisterFunc(GetSavedPalettes);
			} catch (Exception e) {
				PluginServices.Log.Error("Failed to initialise Palette+ IPC", e);
			}
		}

		internal static void Dispose() {
			IApiVersion?.UnregisterFunc();
			IGetCharaPalette?.UnregisterFunc();
			ISetCharaPalette?.UnregisterAction();
			IRemoveCharaPalette?.UnregisterAction();
			IRedrawChara?.UnregisterAction();
			IBuildCharaPalette?.UnregisterFunc();
			IBuildCharaPaletteOrEmpty?.UnregisterFunc();
			IGetSavedPalettes?.UnregisterFunc();
		}

		// IPC Methods

		private static string ApiVersion() => API_VERSION;

		private static string GetCharaPalette(Character chara)
			=> PaletteService.GetCharaPalette(chara).ToString();

		private static void SetCharaPalette(Character chara, string json)
			=> PaletteService.SetCharaPalette(chara, Palette.FromJson(json));

		private static void RemoveCharaPalette(Character chara)
			=> PaletteService.RemoveCharaPalette(chara);

		private unsafe static void RedrawChara(Character actor) {
        actor.Redraw();
    }


    private static string BuildCharaPalette(Character chara) {
			PaletteService.BuildCharaPalette(chara, out var palette, out _, false);
			return palette.ToString();
		}

		private static string BuildCharaPaletteOrEmpty(Character chara) {
			PaletteService.BuildCharaPalette(chara, out var palette, out _, false);
			if (palette == null || palette.ShaderParams.Count == 0) return string.Empty;
			return palette.ToString();
		}

		private static string[] GetSavedPalettes() {
				return PalettePlus.Config.SavedPalettes.Select(JsonConvert.SerializeObject).ToArray();
		}


    internal static void PaletteChanged(Character character, Palette? palette) {
			IPaletteChanged?.SendMessage(character, palette == null ? string.Empty : palette.ToString());
		}

	}
}
