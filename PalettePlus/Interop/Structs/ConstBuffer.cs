using System.Runtime.InteropServices;

namespace PalettePlus.Interop.Structs; 

[StructLayout(LayoutKind.Explicit, Size = 0x70)]
public struct ConstBuffer {
	[FieldOffset(0x00)] public nint __vtable;
	[FieldOffset(0x28)] public nint Values;
	[FieldOffset(0x50)] public unsafe fixed ulong __unkPtr[4];
}