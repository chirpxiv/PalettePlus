using System;
using System.Collections.Generic;

using Dalamud.Game;
using Dalamud.Logging;

namespace PalettePlus.Interop.Hooking; 

// ReSharper disable once ClassNeverInstantiated.Global
public class HookManager : IDisposable {
	private readonly ISigScanner _scanner;
    
	private readonly Dictionary<Type, IHookWrapper> Hooks = new();

	public HookManager(ISigScanner _scanner) {
		this._scanner = _scanner;
	}
	
	// Factory methods

	public HookWrapper<T> AddSignature<T>(T @delegate, string sig) where T : Delegate
		=> AddAddress(this._scanner.ScanText(sig), @delegate);

	public HookWrapper<T> AddAddress<T>(nint address, T @delegate) where T : Delegate {
		var hook = new HookWrapper<T>(address, @delegate);
		this.Hooks.Add(typeof(T), hook);
		return hook;
	}
	
	// Hook acquisition

	public HookWrapper<T>? Get<T>() where T : Delegate {
		this.Hooks.TryGetValue(typeof(T), out var hook);
		return (HookWrapper<T>?)hook;
	}
	
	// Disposal

	public void Dispose() {
		PluginLog.Verbose("Disposing hooks...");
		foreach (var (del, hook) in this.Hooks) {
			try {
				hook.Dispose();
				PluginLog.Verbose($"Disposed hook: {del.Name}");
			} catch (Exception err) {
				PluginLog.Error($"Failed to dispose of hook '{del.Name}':\n{err}");
			}
		}
		this.Hooks.Clear();
	}
}