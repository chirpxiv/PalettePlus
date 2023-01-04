using System;

namespace ColorEdit.Palettes.Attributes {
	// Field attributes for UI display

	[AttributeUsage(AttributeTargets.Field)]
	public class ShowAlpha : Attribute { }

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