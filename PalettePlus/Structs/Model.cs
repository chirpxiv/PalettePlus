﻿using System;
using System.Runtime.InteropServices;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

using Dalamud.Game.ClientState.Objects.Types;

using PalettePlus.Palettes;
using PalettePlus.Services;

namespace PalettePlus.Structs {
	[StructLayout(LayoutKind.Explicit)]
	public struct Model {
		[FieldOffset(0)] public Human Human;

		[FieldOffset(0x9d8)] public unsafe ModelShader* ModelShader;
		[FieldOffset(0x9e0)] public unsafe ModelShader* DecalShader;

		public unsafe ModelParams* GetModelParams() {
			if (ModelShader == null || (nint)ModelShader == 0) return null;
			return ModelShader->ModelParams;
		}

		public unsafe DecalParams* GetDecalParams() {
			if (DecalShader == null || (nint)ModelShader == 0) return null;
			return DecalShader->DecalParams;
		}

		public unsafe static Model* GetModel(GameObject obj) {
			var gameObject = (GameObjectStruct*)obj.Address;
			return gameObject != null ? (Model*)gameObject->DrawObject : null;
		}

		public unsafe ParamContainer GenerateColorValues() {
			var modelParams = new ModelParams();
			var decalParams = new DecalParams();

			if (ModelShader != null && (nint)ModelShader != 0 && DecalShader != null && (nint)DecalShader != 0) {
				fixed (byte* custom = Human.Customize.Data) {
					var model = *ModelShader;
					model.Parameters = (IntPtr)(ModelParams*)&modelParams;

					var decal = *DecalShader;
					decal.Parameters = (IntPtr)(DecalParams*)&decalParams;

					for (var i = 0; i < 4; i++) { // This may be 3 instead of 4.
						model.Pointers[i] = (ulong)model.Parameters;
						decal.Pointers[i] = (ulong)decal.Parameters;
					}

					Interop.Hooks.GenerateColors(CharacterUtility.Instance(), &model, &decal, custom);
				}
			}

			return new ParamContainer {
				Model = modelParams,
				Decal = decalParams
			};
		}

		public unsafe void ApplyPalette(Palette p) {
			var mP = GetModelParams();
			if (mP != null) {
				var o = (object)*mP;
				p.ApplyShaderParams(ref o);
				*mP = (ModelParams)o;
			}

			var dP = GetDecalParams();
			if (dP != null) {
				var o = (object)*dP;
				p.ApplyShaderParams(ref o);
				*dP = (DecalParams)o;
			}
		}
		
		public unsafe void BuildCharaPalette(out Palette palette, out Palette basePalette, bool contain = false) {
			palette = new();
			basePalette = new();

			// TODO: Refactor

			var mP = GetModelParams();
			if (mP != null)
				palette.CopyShaderParams(*mP);

			var dP = GetDecalParams();
			if (dP != null)
				palette.CopyShaderParams(*dP);

			var vals = GenerateColorValues();
			basePalette.CopyShaderParams(vals.Model);
			basePalette.CopyShaderParams(vals.Decal);
			if (contain) PaletteService.ParamContainer = vals;

			foreach (var (key, value) in palette.ShaderParams) {
				if (value.Equals(basePalette.ShaderParams[key]))
					palette.ShaderParams.Remove(key);
			}
		}
	}
}
