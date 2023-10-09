using System;
using System.Linq;
using System.Collections.Generic;

using Dalamud.Hooking;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility.Signatures;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using CSGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

using PalettePlus.Structs;
using PalettePlus.Services;
using PalettePlus.Extensions;
using PalettePlus.Palettes;

namespace PalettePlus.Interop {
	internal class Hooks {
		internal static Dictionary<int, int> ActorCopy = new();

		[Signature("E8 ?? ?? ?? ?? B2 FF 48 8B CB")]
		internal static UpdateColorsDelegate UpdateColors = null!;
		internal unsafe delegate nint UpdateColorsDelegate(Model* a1);
		
		[Signature("48 8B C4 4C 89 40 18 48 89 50 10 55 53")]
		internal static GenerateColorsDelegate GenerateColors = null!;
		internal unsafe delegate nint GenerateColorsDelegate(CharacterUtility* a1, ModelShader* model, ModelShader* decal, byte* customize);

		[Signature("E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 33 45 33 C0", DetourName = nameof(EnableDrawDetour))]		
		private Hook<EnableDrawDelegate> EnableDrawHook = null!;
		private unsafe delegate nint EnableDrawDelegate(CSGameObject* a1);

		[Signature("E8 ?? ?? ?? ?? 0F B6 9F ?? ?? ?? ?? 48 8D 8F", DetourName = nameof(CopyCharaDetour))]
		private Hook<CopyCharaDelegate> CopyCharaHook = null!;
		private unsafe delegate nint CopyCharaDelegate(CharaSetup* to, nint from, uint unk);

		[Signature("E8 ?? ?? ?? ?? 83 BF ?? ?? ?? ?? ?? 75 34", DetourName = nameof(UpdateCharaDetour))]
		private Hook<UpdateCharaDelegate> UpdateCharaHook = null!;
		private delegate bool UpdateCharaDelegate(nint drawObj, nint data, char skipEquip);

		internal void Init() {
			PluginServices.Hooks.InitializeFromAttributes(this);
			
			this.EnableDrawHook.Enable();
			this.CopyCharaHook.Enable();
			this.UpdateCharaHook.Enable();
		}

		internal void Dispose() {
			this.EnableDrawHook.Disable();
			this.EnableDrawHook.Dispose();

			this.CopyCharaHook.Disable();
			this.CopyCharaHook.Dispose();
			
			this.UpdateCharaHook.Disable();
			this.UpdateCharaHook.Dispose();
		}

		private unsafe nint EnableDrawDetour(CSGameObject* a1) {
            var c1 = ((byte)a1->TargetableStatus & 0x80) != 0;
			var c2 = (a1->RenderFlags & 0x2000000) == 0;
			var isNew = !(c1 && c2);

			var exec = EnableDrawHook.Original(a1);

			try {
				if (isNew) {
					var chara = PluginServices.ObjectTable.CreateObjectReference((nint)a1) as Character;
					if (chara != null)
						GetPalette(chara)?.Apply(chara);
				}
			} catch (Exception err) {
				PluginServices.Log.Error($"Failed to load palette for actor:\n{err}");
			}

			return exec;
		}

		private unsafe nint CopyCharaDetour(CharaSetup* to, nint from, uint unk) {
			var exec = CopyCharaHook.Original(to, from, unk);

			try {
				var toAddr = to != null ? (nint)to->Character : nint.Zero;
				
				var toObj = PluginServices.ObjectTable.CreateObjectReference(toAddr) as Character;
				var fromObj = PluginServices.ObjectTable.CreateObjectReference(from) as Character;
				if (toObj != null && fromObj != null && toObj.ObjectIndex >= 200 && toObj.ObjectIndex < 240) {
					PluginServices.Log.Verbose($"Copying from {fromObj.ObjectIndex} to {toObj.ObjectIndex}");
					ActorCopy.Add(toObj.ObjectIndex, fromObj.ObjectIndex);
				}
			} catch (Exception err) {
				PluginServices.Log.Error($"Failed to handle character copy:\n{err}");
			}

			return exec;
		}

		private unsafe bool UpdateCharaDetour(nint drawObj, nint data, char skipEquip) {
			try {
				var owner = GetOwner(drawObj);
				var palette = owner != null ? GetPalette(owner) : null;
				if (palette != null) {
					var model = owner!.GetModel();
					
					model->BuildCharaPalette(out _, out var before);
					var result = UpdateCharaHook.Original(drawObj, data, skipEquip);
					model->BuildCharaPalette(out _, out var after);

					foreach (var key in palette.ShaderParams.Keys) {
						if (!before.ShaderParams[key].Equals(after.ShaderParams[key]))
							palette.ShaderParams.Remove(key);
					}

					palette.Apply(owner!, true);
					
					PluginServices.Log.Verbose($"Re-applying saved palette state for '{owner!.Name}'");

					return result;
				}
			} catch (Exception err) {
				PluginServices.Log.Error($"Failed to handle character update:\n{err}");
			}

			return UpdateCharaHook.Original(drawObj, data, skipEquip);
		}

		private unsafe Character? GetOwner(nint drawObj) {
			var charas = PluginServices.ObjectTable
				.Where(obj => obj is Character)
				.Cast<Character>();

			foreach (var chara in charas) {
				var csPtr = (CSGameObject*)chara.Address;
				if (csPtr != null && drawObj == (nint)csPtr->DrawObject)
					return chara;
			}
			
			return null;
		}

		private Palette? GetPalette(Character chara) {
			if (!chara.IsValidForPalette())
				return null;
			
			Palette? palette = null;

			if (chara.ObjectIndex is >= 200 and < 240) {
				if (ActorCopy.TryGetValue(chara.ObjectIndex, out var fromId)) {
					ActorCopy.Remove(chara.ObjectIndex);

					var addr = PluginServices.ObjectTable.GetObjectAddress(fromId);
					var copyFrom = PluginServices.ObjectTable.CreateObjectReference(addr) as Character;
					if (copyFrom != null)
						palette = PaletteService.GetCharaPalette(copyFrom);
				}
			}

			palette ??= PaletteService.GetCharaPalette(chara, ApplyOrder.StoredFirst);

			return palette;
		}
	}
}
