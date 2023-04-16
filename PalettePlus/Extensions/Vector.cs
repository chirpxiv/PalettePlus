using System;
using System.Numerics;

namespace PalettePlus.Extensions {
	internal static class VectorExtensions {
		internal static void RgbSqrt(this Vector3 vec) {
			vec.X = (float)Math.Sqrt(vec.X);
			vec.Y = (float)Math.Sqrt(vec.Y);
			vec.Z = (float)Math.Sqrt(vec.Z);
		}
		
		internal static void RgbSqrt(this Vector4 vec) {
			vec.X = (float)Math.Sqrt(vec.X);
			vec.Y = (float)Math.Sqrt(vec.Y);
			vec.Z = (float)Math.Sqrt(vec.Z);
		}

		internal static void RgbPow2(this Vector3 vec) {
			vec.X *= vec.X;
			vec.Y *= vec.Y;
			vec.Z *= vec.Z;
		}
		
		internal static void RgbPow2(this Vector4 vec) {
			vec.X *= vec.X;
			vec.Y *= vec.Y;
			vec.Z *= vec.Z;
		}
	}
}