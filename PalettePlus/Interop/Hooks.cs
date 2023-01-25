using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Dalamud.Logging;
using Dalamud.Hooking;
using Dalamud.Game.ClientState.Objects.Types;

using CSGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

using PalettePlus.Structs;
using PalettePlus.Services;
using PalettePlus.Extensions;
using PalettePlus.Palettes;

namespace PalettePlus.Interop {
	internal static class Hooks {
		private const string QWordSig = "4C 8B C0 48 8B 0D ?? ?? ?? ??";
		private const string UpdateColorsSig = "E8 ?? ?? ?? ?? B2 FF 48 8B CB";
		private const string GenerateColorsSig = "48 8B C4 4C 89 40 18 48 89 50 10 55 53";
		private const string EnableDrawSig = "E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 33 45 33 C0";
		private const string CopyCharaSig = "E8 ?? ?? ?? ?? 0F B6 9F ?? ?? ?? ?? 48 8D 8F";

		internal static Dictionary<int, int> ActorCopy = new();

		internal static IntPtr UnknownQWord;

		internal unsafe delegate IntPtr UpdateColorsDelegate(Model* a1);
		internal static UpdateColorsDelegate UpdateColors = null!;
		//internal static Hook<UpdateColorsDelegate> UpdateColorsHook = null!;

		internal unsafe delegate IntPtr GenerateColorsDelegate(IntPtr a1, ModelShader* model, ModelShader* decal, byte* customize);
		internal static GenerateColorsDelegate GenerateColors = null!;

		internal unsafe delegate nint EnableDrawDelegate(CSGameObject* a1);
		internal static Hook<EnableDrawDelegate> EnableDrawHook = null!;

		internal unsafe delegate nint CopyCharaDelegate(nint to, nint from, uint unk);
		internal static Hook<CopyCharaDelegate> CopyCharaHook = null!;

		internal unsafe static void Init() {
			UnknownQWord = *(IntPtr*)PluginServices.SigScanner.GetStaticAddressFromSig(QWordSig);

			var updateColors = PluginServices.SigScanner.ScanText(UpdateColorsSig);
			UpdateColors = Marshal.GetDelegateForFunctionPointer<UpdateColorsDelegate>(updateColors);
			//UpdateColorsHook = Hook<UpdateColorsDelegate>.FromAddress(updateColors, UpdateColorsDetour);
			//UpdateColorsHook.Enable();

			var generateColors = PluginServices.SigScanner.ScanText(GenerateColorsSig);
			GenerateColors = Marshal.GetDelegateForFunctionPointer<GenerateColorsDelegate>(generateColors);

			var enableDraw = PluginServices.SigScanner.ScanText(EnableDrawSig);
			EnableDrawHook = Hook<EnableDrawDelegate>.FromAddress(enableDraw, EnableDrawDetour);
			EnableDrawHook.Enable();

			var copyChara = PluginServices.SigScanner.ScanText(CopyCharaSig);
			CopyCharaHook = Hook<CopyCharaDelegate>.FromAddress(copyChara, CopyCharaDetour);
			CopyCharaHook.Enable();
		}

		internal static void Dispose() {
			//UpdateColorsHook.Disable();
			//UpdateColorsHook.Dispose();

			EnableDrawHook.Disable();
			EnableDrawHook.Dispose();

			CopyCharaHook.Disable();
			CopyCharaHook.Dispose();
		}

		internal unsafe static nint EnableDrawDetour(CSGameObject* a1) {
			var c1 = ((byte)a1->TargetableStatus & 0x40) != 0;
			var c2 = (a1->RenderFlags & 0x2000000) == 0;
			var isNew = !(c1 && c2);

			var exec = EnableDrawHook.Original(a1);

			try {
				if (isNew) {
					var chara = PluginServices.ObjectTable.CreateObjectReference((nint)a1) as Character;

					if (chara != null && chara.IsValidForPalette()) {
						Palette? palette = null;

						if (chara.ObjectIndex >= 200 && chara.ObjectIndex < 240) {
							if (ActorCopy.TryGetValue(chara.ObjectIndex, out var fromId)) {
								ActorCopy.Remove(chara.ObjectIndex);

								var addr = PluginServices.ObjectTable.GetObjectAddress(fromId);
								var copyFrom = PluginServices.ObjectTable.CreateObjectReference(addr) as Character;
								if (copyFrom != null)
									palette = PaletteService.GetCharaPalette(copyFrom);
							}
						}

						if (palette == null)
							palette = PaletteService.GetCharaPalette(chara, ApplyOrder.StoredFirst);

						palette.Apply(chara);
					}
				}
			} catch (Exception e) {
				PluginLog.Error("Failed to load palette for actor", e);
			}

			return exec;
		}

		internal unsafe static nint CopyCharaDetour(nint to, nint from, uint unk) {
			var exec = CopyCharaHook.Original(to, from, unk);

			try {
				var toObj = PluginServices.ObjectTable.CreateObjectReference(to) as Character;
				var fromObj = PluginServices.ObjectTable.CreateObjectReference(from) as Character;
				if (toObj != null && fromObj != null && toObj.ObjectIndex >= 200 && toObj.ObjectIndex < 240) {
					PluginLog.Verbose($"Copying from {fromObj.ObjectIndex} to {toObj.ObjectIndex}");
					ActorCopy.Add(toObj.ObjectIndex, fromObj.ObjectIndex);
				}
			} catch (Exception e) {
				PluginLog.Error("Failed to handle character copy", e);
			}

			return exec;
		}
	}
}