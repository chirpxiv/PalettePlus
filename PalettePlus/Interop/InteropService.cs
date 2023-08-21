using System;

using PalettePlus.Core;
using PalettePlus.Interop.Hooking;

namespace PalettePlus.Interop;

// Delegate declarations
// TODO

// ReSharper disable once ClassNeverInstantiated.Global
public class InteropService : IDisposable {
	// Hooks
	
	private readonly HookManager Hooks;
	
	// Native functions
	
	// Service initialization
	
	public InteropService(IServiceContainer _services) {
		this.Hooks = _services.Inject<HookManager>();
	}
	
	// Disposal

	public void Dispose() {
		this.Hooks.Dispose();
	}
}