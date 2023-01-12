using System;
using System.Reflection;

namespace PalettePlus.Palettes.Attributes {
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

	[AttributeUsage(AttributeTargets.Field)]
	public class Conditional : Attribute {
		public PaletteConditions Conditions;

		public Conditional(PaletteConditions conditions)
			=> Conditions = conditions;
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalLink : Conditional {
		public string LinkedTo;

		public FieldInfo? LinkedField;

		public bool LastResult = false;

		public ConditionalLink(PaletteConditions conditions, string linkedTo) : base(conditions) {
			LinkedTo = linkedTo;
		}
	}
}