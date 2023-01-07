using System;
using System.Collections.Generic;

using Dalamud.Configuration;
using Dalamud.Logging;

using ColorEdit.Palettes;
using ColorEdit.Interface;
using ColorEdit.Interface.Windows;

namespace ColorEdit {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public const int _ConfigVer = 0;

		public int Version { get; set; } = _ConfigVer;
		public string PluginVersion { get; set; } = ColorEdit.GetVersion();

		public bool IsFirstTime { get; set; } = true;

		public Dictionary<string, Palette> SavedPalettes { get; set; } = new();

		public static void LoadConfig() {
			try {
				ColorEdit.Config = Services.Interface.GetPluginConfig() as Configuration ?? new();
			} catch(Exception e) {
				PluginLog.Error("Failed to load ColorEdit config. Settings have been reset.", e);
				ColorEdit.Config = new();
			}
			ColorEdit.Config.Init();
		}

		public void Init() {
			if (Version != _ConfigVer)
				Upgrade();

			var curVer = ColorEdit.GetVersion();
			if (PluginVersion != curVer) {
				// TODO: Changelog?
				PluginVersion = curVer;
			}

			if (IsFirstTime) {
				IsFirstTime = false;
				PluginGui.GetWindow<MainWindow>().Show();
			}
		}

		private void Upgrade() {
			PluginLog.Warning(string.Format(
				"Upgrading config version from {0} to {1}.\nThis is nothing to worry about, but some settings may change or reset!",
				Version, _ConfigVer
			));

			Version = _ConfigVer;
		}
	}
}