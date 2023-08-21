using System;
using System.Reflection;
using System.Collections.Generic;

using Dalamud.Interface;
using Dalamud.Interface.Windowing;

using PalettePlus.Core;
using PalettePlus.Interface.Windows;

namespace PalettePlus.Interface;

// ReSharper disable once ClassNeverInstantiated.Global
public class GuiService : IDisposable {
	// Dependencies
	
	private readonly IServiceContainer _services;
	private readonly UiBuilder _uiBuilder;
	
	// Reflection cache

	private readonly static FieldInfo WindowsField = typeof(WindowSystem)
		.GetField("windows", BindingFlags.Instance | BindingFlags.NonPublic)!;
	
	// Window system
	
	private readonly WindowSystem Windows = new("PalettePlus");

	private List<Window> WindowList => (List<Window>)WindowsField.GetValue(this.Windows)!;
	
	// Service constructor
	
	public GuiService(IServiceContainer _services, UiBuilder _uiBuilder) {
		this._services = _services;
		this._uiBuilder = _uiBuilder;
        
		_uiBuilder.Draw += this.Windows.Draw;
		_uiBuilder.OpenConfigUi += ToggleMainWindow;
		_uiBuilder.DisableGposeUiHide = true;
	}
	
	// Window access

	public T GetWindow<T>() where T : Window {
		if (this.WindowList.Find(w => w is T) is T result)
			return result;

		var window = this._services.Inject<T>();
		this.Windows.AddWindow(window);
		return window;
	}
	
	// Main window toggle

	public void ToggleMainWindow()
		=> this.GetWindow<MainWindow>().Toggle();
	
	// Disposal

	public void Dispose() {
		this._uiBuilder.Draw -= this.Windows.Draw;
		this._uiBuilder.OpenConfigUi -= ToggleMainWindow;
	}
}