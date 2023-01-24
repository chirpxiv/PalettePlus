using Dalamud.Plugin.Ipc;

using PalettePlus.Services;

namespace PalettePlus.Interop {
	internal class IpcProvider {
		internal static readonly string API_VERSION = "1.0.0";

		internal static readonly string ApiVersionId = "PalettePlus.ApiVersion";

		private ICallGateProvider<string>? ApiVersion;

		internal IpcProvider() {
			ApiVersion = PluginServices.Interface.GetIpcProvider<string>(ApiVersionId);
			ApiVersion.RegisterFunc(() => API_VERSION);
		}
	}
}