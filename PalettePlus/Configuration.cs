using System;
using System.Collections.Generic;

using Dalamud.Logging;
using Dalamud.Configuration;

using PalettePlus.Services;
using PalettePlus.Palettes;
using PalettePlus.Interface;
using PalettePlus.Interface.Windows;

namespace PalettePlus {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public const int _ConfigVer = 0;

		public int Version { get; set; } = _ConfigVer;
		public string PluginVersion = PalettePlus.GetVersion();

		public bool IsFirstTime = true;

		public List<Palette> SavedPalettes = new();
		public List<Persist> Persistence = new();

		// Methods

		public void Init() {
			if (Version != _ConfigVer)
				Upgrade();

			var curVer = PalettePlus.GetVersion();
			if (PluginVersion != curVer) {
				// TODO: Changelog?
				PluginVersion = curVer;
			}

			if (IsFirstTime) {
				IsFirstTime = false;
				PluginGui.GetWindow<MainWindow>().Show();
			}
		}

		public void Save() => PluginServices.Interface.SavePluginConfig(this);

		private void Upgrade() {
			PluginLog.Warning(string.Format(
				"Upgrading config version from {0} to {1}.\nThis is nothing to worry about, but some settings may change or reset!",
				Version, _ConfigVer
			));

			Version = _ConfigVer;
		}

		public static void LoadConfig() {
			try {
				PalettePlus.Config = PluginServices.Interface.GetPluginConfig() as Configuration ?? new();
			} catch (Exception e) {
				PluginLog.Error("Failed to load ColorEdit config. Settings have been reset.", e);
				PalettePlus.Config = new();
			}
			PalettePlus.Config.Init();
		}
	}
}