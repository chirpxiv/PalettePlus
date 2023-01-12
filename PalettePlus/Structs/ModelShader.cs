using System;
using System.Numerics;
using System.Runtime.InteropServices;

using PalettePlus.Palettes;
using PalettePlus.Palettes.Attributes;

namespace PalettePlus.Structs {
	[StructLayout(LayoutKind.Explicit, Size = 0x70)]
	public struct ModelShader {
		[FieldOffset(0x00)] public IntPtr VTable;
		[FieldOffset(0x20)] public int Unknown2;
		[FieldOffset(0x24)] public int Unknown3;
		[FieldOffset(0x28)] public IntPtr Parameters;
		[FieldOffset(0x30)] public int Unknown4;
		[FieldOffset(0x34)] public int Unknown5;
		[FieldOffset(0x50)] public unsafe fixed ulong Pointers[4];

		public unsafe ModelParams* ModelParams => (ModelParams*)Parameters;
		public unsafe DecalParams* DecalParams => (DecalParams*)Parameters;
	}

	// Shader parameters for model & decal

	public struct ModelParams {
		public Vector3 SkinTone;
		[Slider(-10, 10)] public float MuscleTone;
		public Vector4 SkinGloss;
		[ShowAlpha] public Vector4 LipColor;
		public Vector4 HairColor;
		public Vector4 HairShine;
		[ConditionalLink(PaletteConditions.Highlights, "HairColor")] public Vector4 HighlightsColor;
		public Vector3 LeftEyeColor;
		[Slider(-10, 10)] public float FacePaintWidth;
		[ConditionalLink(PaletteConditions.Heterochromia, "LeftEyeColor")] public Vector3 RightEyeColor;
		[Slider(-10, 10)] public float FacePaintOffset;
		public Vector4 RaceFeatureColor;

		public ModelParams() {
			SkinTone = new Vector3(1, 1, 1);
			FacePaintWidth = 1f;
			MuscleTone = 1f;
		}
	}

	public struct DecalParams {
		[ShowAlpha] public Vector4 FacePaintColor;
	}
}