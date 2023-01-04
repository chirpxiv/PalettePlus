using System;

using Dalamud.Hooking;

using ColorEdit.Structs;

namespace ColorEdit.Interop {
	internal static class Hooks {
		private const string UpdateColorsSig = "E8 ?? ?? ?? ?? B2 FF 48 8B CB";

		internal unsafe delegate IntPtr UpdateColorsDelegate(Model* a1);
		internal static Hook<UpdateColorsDelegate> UpdateColorsHook = null!;
		//internal static UpdateColorsDelegate UpdateColors = null!;

		internal unsafe static void Init() {
			var addr = Services.SigScanner.ScanText(UpdateColorsSig);
			//UpdateColors = Marshal.GetDelegateForFunctionPointer<UpdateColorsDelegate>(addr);
			UpdateColorsHook = Hook<UpdateColorsDelegate>.FromAddress(addr, UpdateColorsDetour);
			UpdateColorsHook.Enable();
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