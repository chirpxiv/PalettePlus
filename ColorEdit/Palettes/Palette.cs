using System.Collections.Generic;

using ColorEdit.Structs;

namespace ColorEdit.Palettes {
	public class Palette : Dictionary<string, object> {
		public void SetValue(string key, object value, bool exists) {
			if (exists)
				this[key] = value;
			else
				Add(key, value);
		}

		public void SetValue(string key, object value)
			=> SetValue(key, value, ContainsKey(key));

		public void Copy(ColorData data) {
			var fields = typeof(ColorData).GetFields();
			foreach (var field in fields)
				SetValue(field.Name, field.GetValue(data)!, false);
		}

		public void Apply(ref object data) {
			if (data is ColorData == false) return;
			foreach (var (name, value) in this) {
				var field = typeof(ColorData).GetField(name)!;
				field.SetValue(data, value);
			}
		}
	}
}