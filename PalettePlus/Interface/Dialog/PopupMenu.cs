using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Interface.Windowing;

namespace PalettePlus.Interface.Dialog {
	// Repurposed code from Ktisis 0.3. A commons library would be kinda neat.

	public class PopupMenu : Window {
		private Action? DrawCallback;

		public PopupMenu() : base(
			"##PPContextMenu",
			ImGuiWindowFlags.NoDecoration
			^ ImGuiWindowFlags.NoMove
			^ ImGuiWindowFlags.AlwaysAutoResize
		) {}

		public void Open(Action? draw) {
			DrawCallback = draw;
			IsOpen = true;
		}

		public override void Draw() {
			DrawCallback?.Invoke(); // sorry, I'm a lazy bitch

			if (!ImGui.IsWindowFocused())
				IsOpen = false;
		}

		public override void OnOpen() {
			base.OnOpen();
			Position = ImGui.GetMousePos() + new Vector2(5, 0);
			ImGui.SetNextWindowPos((Vector2)Position);
			ImGui.SetNextWindowFocus();
		}

		public override void OnClose() {
			DrawCallback = null;
			base.OnClose();
		}
	}
}