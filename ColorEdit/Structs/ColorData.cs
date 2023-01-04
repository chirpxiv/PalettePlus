using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ColorEdit.Structs {
	public struct ColorData {
		public Vector3 SkinTone;
		[Slider(-10, 10)] public float MuscleTone;
		public Vector4 SkinGloss;
		[ShowAlpha] public Vector4 LipColor;
		public Vector4 HairColor;
		public Vector4 HairShine;
		public Vector4 HighlightsColor;
		public Vector3 LeftEyeColor;
		[Slider(-1, 1)] public float FacePaintWidth;
		public Vector3 RightEyeColor;
		[Slider(-1, 1)] public float FacePaintOffset;
		public Vector4 RaceFeatureColor;
	}

	// unsure what this struct actually is, but it has the pointer we need
	[StructLayout(LayoutKind.Explicit)]
	public struct ModelData {
		[FieldOffset(0x00)] public IntPtr Unknown1;
		[FieldOffset(0x28)] public unsafe ColorData* ColorData;
	}

	// Field attributes for UI display

	[AttributeUsage(AttributeTargets.Field)]
	public class ShowAlpha : Attribute {}

	[AttributeUsage(AttributeTargets.Field)]
	public class Slider : Attribute {
		public float Min;
		public float Max;

		public Slider(float min, float max) { 
			Min = min;
			Max = max;
		}
	}
}