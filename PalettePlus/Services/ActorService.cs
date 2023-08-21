using System.Collections.Generic;

using Dalamud.Plugin.Services;
using Dalamud.Game.ClientState.Objects.Types;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using CSCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

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
	
	public IEnumerable<GameObject> EnumerateAll() {
		for (var i = 0; i < MaxCharaIndex; i++) {
			var actor = this._objects[i];
			if (actor is null) continue;
			yield return actor;
		}
	}

	public IEnumerable<Character> EnumerateHumans() {
		foreach (var gameObj in EnumerateAll()) {
			if (gameObj is Character chara && IsHuman(chara))
				yield return chara;
		}
	}
	
	// Helpers

	private const int HumanModelId = 0;
	
	public unsafe bool IsHuman(Character chara) {
		var csPtr = (CSCharacter*)chara.Address;
		if (csPtr == null) return false;
		
		// Match model ID

		var data = csPtr->CharacterData;
		var modelId = data.ModelCharaId_2 != -1 ? data.ModelCharaId_2 : data.ModelCharaId;
		if (modelId != HumanModelId)
			return false;
		
		// Match object type & model type

		var drawPtr = csPtr->GameObject.DrawObject;
		if (drawPtr == null || drawPtr->Object.GetObjectType() != ObjectType.CharacterBase)
			return false;

		return ((CharacterBase*)drawPtr)->GetModelType() == CharacterBase.ModelType.Human;
	}

	public bool IsGPoseActor(Character chara)
		=> IsGPoseActor(chara.ObjectIndex);

	public bool IsGPoseActor(uint id)
		=> id is >= GPoseIndex and < MaxCharaIndex;
}