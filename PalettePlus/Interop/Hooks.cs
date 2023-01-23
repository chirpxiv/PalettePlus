using System;
using System.Runtime.InteropServices;

using Dalamud.Hooking;

using FFXIVClientStructs.FFXIV.Client.Game.Object;

using PalettePlus.Structs;
using PalettePlus.Services;
using PalettePlus.Extensions;

namespace PalettePlus.Interop {
	internal static class Hooks {
		private const string QWordSig = "4C 8B C0 48 8B 0D ?? ?? ?? ??";
		private const string UpdateColorsSig = "E8 ?? ?? ?? ?? B2 FF 48 8B CB";
		private const string GenerateColorsSig = "48 8B C4 4C 89 40 18 48 89 50 10 55 53";

		internal static IntPtr UnknownQWord;

		internal unsafe delegate IntPtr UpdateColorsDelegate(Model* a1);
		internal static UpdateColorsDelegate UpdateColors = null!;
		//internal static Hook<UpdateColorsDelegate> UpdateColorsHook = null!;

		internal unsafe delegate IntPtr GenerateColorsDelegate(IntPtr a1, ModelShader* model, ModelShader* decal, byte* customize);
		internal static GenerateColorsDelegate GenerateColors = null!;

		internal unsafe delegate nint EnableDrawDelegate(GameObject* a1);
		internal static Hook<EnableDrawDelegate> EnableDrawHook = null!;

		internal unsafe static void Init() {
			UnknownQWord = *(IntPtr*)PluginServices.SigScanner.GetStaticAddressFromSig(QWordSig);

			var updateColors = PluginServices.SigScanner.ScanText(UpdateColorsSig);
			UpdateColors = Marshal.GetDelegateForFunctionPointer<UpdateColorsDelegate>(updateColors);
			//UpdateColorsHook = Hook<UpdateColorsDelegate>.FromAddress(updateColors, UpdateColorsDetour);
			//UpdateColorsHook.Enable();

			var generateColors = PluginServices.SigScanner.ScanText(GenerateColorsSig);
			GenerateColors = Marshal.GetDelegateForFunctionPointer<GenerateColorsDelegate>(generateColors);

			var test = PluginServices.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 33 45 33 C0");
			EnableDrawHook = Hook<EnableDrawDelegate>.FromAddress(test, EnableDrawDetour);
			EnableDrawHook.Enable();
		}

		internal static void Dispose() {
			//UpdateColorsHook.Disable();
			//UpdateColorsHook.Dispose();

			EnableDrawHook.Disable();
			EnableDrawHook.Dispose();
		}

		internal unsafe static nint EnableDrawDetour(GameObject* a1) {
			var c1 = (*(byte*)((nint)a1 + 149) & 0x40) != 0;
			var c2 = (a1->RenderFlags & 0x2000000) == 0;
			var isNew = !(c1 && c2);

			var exec = EnableDrawHook.Original(a1);

			if (isNew) {
				var obj = PluginServices.ObjectTable.CreateObjectReference((nint)a1);
				if (obj != null && obj.Name != null && ((Actor*)a1)->ModelId == 0) {
					var palettes = obj.GetPersists();
					foreach (var palette in palettes)
						palette.Apply(obj);
				}
			}

			return exec;
		}

		/*internal unsafe static IntPtr UpdateColorsDetour(Model* model) {
			var exec = UpdateColorsHook.Original(model);
			return exec;
		}*/
	}
}