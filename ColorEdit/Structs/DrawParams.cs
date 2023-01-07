using System;
using System.Numerics;
using System.Runtime.InteropServices;

using ColorEdit.Palettes.Attributes;

namespace ColorEdit.Structs {
	public struct DrawParams {
		public Vector3 SkinTone;
		[Slider(-10, 10)] public float MuscleTone;
		public Vector4 SkinGloss;
		[ShowAlpha] public Vector4 LipColor;
		public Vector4 HairColor;
		public Vector4 HairShine;
		public Vector4 HighlightsColor;
		public Vector3 LeftEyeColor;
		[Slider(-10, 10)] public float FacePaintWidth;
		[Linked(LinkType.Eyes, "LeftEyeColor")] public Vector3 RightEyeColor;
		[Slider(-10, 10)] public float FacePaintOffset;
		public Vector4 RaceFeatureColor;
	}

	// unsure what this struct actually is, but it has the pointer we need
	[StructLayout(LayoutKind.Explicit)]
	public struct ModelData {
		[FieldOffset(0x00)] public IntPtr Unknown1;
		[FieldOffset(0x28)] public unsafe DrawParams* ColorData;
	}
}