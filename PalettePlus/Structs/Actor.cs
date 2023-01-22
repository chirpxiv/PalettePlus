using System.Runtime.InteropServices;

using Dalamud.Game.ClientState.Objects.Types;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace PalettePlus.Structs {
	[StructLayout(LayoutKind.Explicit)]
	public struct Actor {
		[FieldOffset(0)] internal GameObjectStruct GameObject;

		// TODO: ClientStructs PR
		[FieldOffset(0x1B4)] public uint ModelId;

		public unsafe Model* GetModel()
			=> (Model*)GameObject.DrawObject;

		public unsafe static Actor* GetActor(GameObject obj)
			=> (Actor*)obj.Address;
	}
}