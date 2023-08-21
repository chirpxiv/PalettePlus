using System.Numerics;

namespace PalettePlus.Interop.Structs; 

public struct ModelParams {
	public Vector3 SkinTone;
	public float MuscleTone;
	public Vector4 SkinGloss;
	public Vector4 LipColor;
	public Vector4 HairColor;
	public Vector4 HairShine;
	public Vector4 HighlightsColor;
	public Vector3 EyeColorLeft;
	public float FacepaintWidth;
	public Vector3 EyeColorRight;
	public float FacepaintOffset;
	public Vector4 FeatureColor;
}