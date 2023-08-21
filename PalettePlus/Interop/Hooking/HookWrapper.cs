using System;

using Dalamud.Hooking;

namespace PalettePlus.Interop.Hooking;

public interface IHookWrapper : IDalamudHook, IDisposable {
	public void Enable();
	public void Disable();
}

public class HookWrapper<T> : IHookWrapper where T : Delegate {
	private readonly Hook<T> Hook;

	public nint Address => this.Hook.Address;
	public bool IsEnabled => this.Hook.IsEnabled;
	public bool IsDisposed => this.Hook.IsDisposed;
	public string BackendName => this.Hook.BackendName;
	
	public void Enable() => this.Hook.Enable();
	public void Disable() => this.Hook.Disable();

	public void Dispose() {
		this.Hook.Disable();
		this.Hook.Dispose();
	}

	public HookWrapper(nint address, T @delegate) {
		this.Hook = Hook<T>.FromAddress(address, @delegate);
	}
}