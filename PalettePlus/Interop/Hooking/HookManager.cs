using System;
using System.Collections.Generic;

using Dalamud.Game;

namespace PalettePlus.Interop.Hooking; 

// ReSharper disable once ClassNeverInstantiated.Global
public class HookManager : IDisposable {
	private readonly ISigScanner _scanner;
    
	private readonly Dictionary<Type, IHookWrapper> Hooks = new();

	public HookManager(ISigScanner _scanner) {
		this._scanner = _scanner;
	}
	
	// Factory methods

	public HookWrapper<T> AddSignature<T>(T @delegate, string sig) where T : Delegate {
		var addr = this._scanner.ScanText(sig);
		return this.AddAddress(addr, @delegate);
	}

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
		foreach (var (_, hook) in this.Hooks)
			hook.Dispose();
		this.Hooks.Clear();
	}
}