using System;
using System.Runtime.InteropServices;

using Dalamud.Hooking;

using PalettePlus.Structs;

namespace PalettePlus.Interop {
	internal static class Hooks {
		private const string QWordSig = "4C 8B C0 48 8B 0D ?? ?? ?? ??";
		private const string UpdateColorsSig = "E8 ?? ?? ?? ?? B2 FF 48 8B CB";
		private const string GenerateColorsSig = "48 8B C4 4C 89 40 18 48 89 50 10 55 53";

		internal static IntPtr UnknownQWord;

		internal unsafe delegate IntPtr UpdateColorsDelegate(Model* a1);
		internal static Hook<UpdateColorsDelegate> UpdateColorsHook = null!;

		internal unsafe delegate IntPtr GenerateColorsDelegate(IntPtr a1, ModelShader* model, ModelShader* decal, byte* customize);
		internal static GenerateColorsDelegate GenerateColors = null!;

		internal unsafe static void Init() {
			UnknownQWord = *(IntPtr*)Services.SigScanner.GetStaticAddressFromSig(QWordSig);

			var updateColors = Services.SigScanner.ScanText(UpdateColorsSig);
			UpdateColorsHook = Hook<UpdateColorsDelegate>.FromAddress(updateColors, UpdateColorsDetour);
			UpdateColorsHook.Enable();

			var generateColors = Services.SigScanner.ScanText(GenerateColorsSig);
			GenerateColors = Marshal.GetDelegateForFunctionPointer<GenerateColorsDelegate>(generateColors);
		}

		internal static void Dispose() {
			UpdateColorsHook.Disable();
			UpdateColorsHook.Dispose();
		}

		internal unsafe static IntPtr UpdateColorsDetour(Model* model) {
			var exec = UpdateColorsHook.Original(model);
			return exec;
		}
	}
}