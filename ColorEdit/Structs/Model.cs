using System;
using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Objects.Types;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace ColorEdit.Structs {
	[StructLayout(LayoutKind.Explicit)]
	public struct Model {
		[FieldOffset(0)] public Human Human;

		[FieldOffset(0x9d8)] public unsafe ModelShader* ModelShader;
		[FieldOffset(0x9e0)] public unsafe ModelShader* DecalShader;

		public unsafe ModelParams* GetColorData() {
			if (ModelShader == null) return null;
			return ModelShader->ModelParams;
		}

		public unsafe static Model* GetModel(GameObject obj) {
			var gameObject = (GameObjectStruct*)obj.Address;
			if (gameObject == null) return null;
			return (Model*)gameObject->DrawObject;
		}

		public unsafe ColorValuePair GenerateColorValues() {
			var modelParams = new ModelParams();
			var decalParams = new DecalParams();

			fixed (byte* custom = Human.CustomizeData) {
				var model = *ModelShader;
				model.Parameters = (IntPtr)(ModelParams*)&modelParams;

				var decal = *DecalShader;
				decal.Parameters = (IntPtr)(DecalParams*)&decalParams;

				for (var i = 0; i < 4; i++) { // This may be 3 instead of 4.
					model.Pointers[i] = (ulong)model.Parameters;
					decal.Pointers[i] = (ulong)decal.Parameters;
				}

				Interop.Hooks.GenerateColors!(Interop.Hooks.UnknownQWord, &model, &decal, custom);
			}

			return new ColorValuePair {
				Model = modelParams,
				Decal = decalParams
			};
		}

		public class ColorValuePair {
			public ModelParams Model;
			public DecalParams Decal;
		}
	}
}