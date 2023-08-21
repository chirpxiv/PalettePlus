using System.Runtime.InteropServices;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

namespace PalettePlus.Interop.Structs; 

[StructLayout(LayoutKind.Explicit)]
public struct CharaModel {
	[FieldOffset(0)] private Human Inner;

	[FieldOffset(0x9D8)] public unsafe ConstBuffer<ModelParams>* ModelBuffer;
	[FieldOffset(0x9E0)] public unsafe ConstBuffer<DecalParams>* DecalBuffer;

	public unsafe ModelParams* ModelParams => this.ModelBuffer != null ? this.ModelBuffer->Data : null;
	public unsafe DecalParams* DecalParams => this.DecalBuffer != null ? this.DecalBuffer->Data : null;
}