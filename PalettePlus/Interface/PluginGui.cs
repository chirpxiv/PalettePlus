using System;
using System.Reflection;
using System.Collections.Generic;

using Dalamud.Interface.Windowing;

namespace PalettePlus.Interface {
	public static class PluginGui {
		public static WindowSystem Windows = new("ColorEdit");

		private static readonly FieldInfo WindowsField = typeof(WindowSystem).GetField("windows", BindingFlags.Instance | BindingFlags.NonPublic)!;
		private static List<Window> WindowsList => (List<Window>?)WindowsField.GetValue(Windows)!;

		public static Window GetWindow<T>(object[]? args = null) {
			foreach (var w in WindowsList)
				if (w is T) return w;

			var window = (Window)Activator.CreateInstance(typeof(T), args)!;
			Windows.AddWindow(window);
			return window;
		}

		public static void RemoveWindow<T>() {
			foreach (var w in WindowsList)
				if (w is T) Windows.RemoveWindow(w);
		}
	}

	internal static class WindowExtensions {
		internal static void Show(this Window window)
			=> window.IsOpen = true;

		internal static void Close(this Window window)
			=> window.IsOpen = false;
	}
}