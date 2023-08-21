using System;

namespace PalettePlus.Core; 

public interface IServiceContainer : IServiceProvider {
	public T? GetService<T>();
	public T GetRequiredService<T>();

	public T Inject<T>();
	public bool Inject<T>(T inst);
}