using PalettePlus.Interop;

namespace PalettePlus.Services; 

public class PaletteService {
	private readonly InteropService _interop;
	
	public PaletteService(InteropService _interop) {
		this._interop = _interop;
	}
}