using System;
using System.Numerics;

namespace PalettePlus.Extensions {
	internal static class VectorExtensions {
		internal static Vector3 RgbSqrt(this Vector3 vec) => new(
			(float)Math.Sqrt(vec.X),
			(float)Math.Sqrt(vec.Y),
			(float)Math.Sqrt(vec.Z)
		);

		internal static Vector4 RgbSqrt(this Vector4 vec) => new(
			(float)Math.Sqrt(vec.X),
			(float)Math.Sqrt(vec.Y),
			(float)Math.Sqrt(vec.Z),
			vec.W
		);

		internal static Vector3 RgbPow2(this Vector3 vec) => vec * vec;

		internal static Vector4 RgbPow2(this Vector4 vec) => new(
			vec.X * vec.X,
			vec.Y * vec.Y,
			vec.Z * vec.Z,
			vec.W
		);
	}
}