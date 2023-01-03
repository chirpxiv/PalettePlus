using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ColorEdit.Structs {
	public struct ColorSets {
		public Vector3 SkinTone;
		public float MuscleTone;
		public Vector4 SkinGloss;
		public Vector4 LipColor;
		public Vector4 HairColor;
		public Vector4 HairShine;
		public Vector4 HighlightsColor;
		public Vector4 LeftEye;
		public Vector4 RightEye;
		public Vector4 LimbalRingColor;
	}

	// unsure what this struct actually is, but it has the pointer we need
	[StructLayout(LayoutKind.Explicit)]
	public struct ModelData {
		[FieldOffset(0x00)] public IntPtr Unknown1;
		[FieldOffset(0x28)] public unsafe ColorSets* ColorData;
	}
}