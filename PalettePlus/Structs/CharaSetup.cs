using System.Runtime.InteropServices;

using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace PalettePlus.Structs; 

[StructLayout(LayoutKind.Explicit, Size = 0x15)]
public struct CharaSetup {
	[FieldOffset(0x08)] public unsafe Character* Character;
	[FieldOffset(0x10)] public byte Unknown1;
	[FieldOffset(0x11)] public uint Unknown2;
}
