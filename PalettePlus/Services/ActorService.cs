using System.Collections.Generic;

using Dalamud.Plugin.Services;
using Dalamud.Game.ClientState.Objects.Types;

namespace PalettePlus.Services; 

// ReSharper disable once ClassNeverInstantiated.Global
public class ActorService {
    // Dependencies

	private readonly IObjectTable _objects;
	
	// Service initialization
	
	public ActorService(IObjectTable _objects) {
		this._objects = _objects;
	}
	
	// Iteration

	private const int GPoseIndex = 201;
	private const int MaxCharaIndex = GPoseIndex + 40;
	
	public IEnumerable<GameObject> GetEnumerator() {
		for (var i = 0; i < MaxCharaIndex; i++) {
			var actor = this._objects[i];
			if (actor is null) continue;
            yield return actor;
		}
	}
}