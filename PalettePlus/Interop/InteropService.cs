using System;

using Dalamud.Utility.Signatures;

using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;

using PalettePlus.Core;
using PalettePlus.Interop.Hooking;

namespace PalettePlus.Interop;

// Delegate declarations

public unsafe delegate bool TextureUploadDelegate(Texture* texture, nint contents);
public unsafe delegate Texture* TextureCreate2DDelegate(Device* device, int* size, byte mipLevel, uint texFormat, uint flags, uint unk);

// ReSharper disable once ClassNeverInstantiated.Global
public class InteropService : IDisposable {
	// Hooks
	
	private readonly HookManager Hooks;
	
	// Native functions

	[Signature("E9 ?? ?? ?? ?? 8B 02 25")]
	public TextureUploadDelegate TextureUpload = null!;

	[Signature("E8 ?? ?? ?? ?? 8B 0F 48 8D 54 24")]
	public TextureCreate2DDelegate TextureCreate2D = null!;
	
	// Service initialization
	
	public InteropService(IServiceContainer _services) {
		this.Hooks = _services.Inject<HookManager>();
		SignatureHelper.Initialise(this);
	}
	
	// Disposal

	public void Dispose() {
		this.Hooks.Dispose();
	}
}