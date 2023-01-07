﻿using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Objects.Types;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace ColorEdit.Structs {
	[StructLayout(LayoutKind.Explicit)]
	public struct Model {
		[FieldOffset(0)] public DrawObject DrawObject;

		[FieldOffset(0x9d8)] public unsafe ModelData* ModelData;

		public unsafe DrawParams* GetColorData() {
			if (ModelData == null) return null;
			return ModelData->ColorData;
		}

		public unsafe static Model* GetModel(GameObject obj) {
			var gameObject = (GameObjectStruct*)obj.Address;
			if (gameObject == null) return null;
			return (Model*)gameObject->DrawObject;
		}
	}
}