using System;
using System.Numerics;

namespace PalettePlus.Extensions {
	internal static class VectorExtensions {
		private static float RgbSqrt(float x)
			=> x < 0.0f ? -MathF.Sqrt(-x) : MathF.Sqrt(x);

		internal static Vector3 RgbSqrt(this Vector3 vec) => new(
			RgbSqrt(vec.X),
			RgbSqrt(vec.Y),
			RgbSqrt(vec.Z)
		);

		internal static Vector4 RgbSqrt(this Vector4 vec) => new(
			RgbSqrt(vec.X),
			RgbSqrt(vec.Y),
			RgbSqrt(vec.Z),
			vec.W
		);

		private static float RgbPow2(float x)
			=> x < 0.0f ? -(x * x) : x * x;

		internal static Vector3 RgbPow2(this Vector3 vec) => new(
			RgbPow2(vec.X),
			RgbPow2(vec.Y),
			RgbPow2(vec.Z)
		);

		internal static Vector4 RgbPow2(this Vector4 vec) => new(
			RgbPow2(vec.X),
			RgbPow2(vec.Y),
			RgbPow2(vec.Z),
			vec.W
		);
	}
}