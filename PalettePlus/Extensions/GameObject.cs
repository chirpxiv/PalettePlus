using System.Linq;
using System.Collections.Generic;

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using CSCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

using PalettePlus.Interop;
using PalettePlus.Structs;
using PalettePlus.Palettes;
using PalettePlus.Services;

namespace PalettePlus.Extensions {
	internal static class CharacterExtensions {
		private unsafe static CSCharacter* GetStruct(this Character actor)
			=> (CSCharacter*)actor.Address;

		internal unsafe static Model* GetModel(this Character actor)
			=> actor.HasHumanModel() ? (Model*)actor.GetStruct()->GameObject.DrawObject : null;

		internal unsafe static bool HasHumanModel(this Character actor) {
			var chara = actor.GetStruct();
			if (chara == null) return false;
			
			var modelId = chara->CharacterData.ModelCharaId_2 != -1 ? chara->CharacterData.ModelCharaId_2 : chara->CharacterData.ModelCharaId;
			if (modelId != 0) return false;
			
			var drawObject = chara->GameObject.DrawObject;
			if (drawObject == null || drawObject->Object.GetObjectType() != ObjectType.CharacterBase) return false;

			return ((CharacterBase*)drawObject)->GetModelType() == CharacterBase.ModelType.Human;
		}
		
		internal unsafe static ModelParams* UpdateColors(this Character actor) {
			var model = actor.GetModel();
			if (model == null) return null;

			Hooks.UpdateColors(model);

			return model->GetModelParams();
		}

		internal static List<Palette> GetPersists(this Character actor) {
			var results = new List<Palette>();

			foreach (var persist in PalettePlus.Config.Persistence) {
				if (!persist.Enabled || !persist.IsApplicableTo(actor)) continue;

				var palette = persist.FindPalette();
				if (palette != null) results.Add(palette);
			}

			return results;
		}

		internal static bool IsValidForPalette(this Character obj)
			=> obj.ObjectKind != ObjectKind.EventNpc && obj.HasHumanModel();

		internal static Character? FindOverworldEquiv(this Character obj)
			=> PluginServices.ObjectTable.FirstOrDefault(ch => ch.ObjectIndex < 200 && ch is Character && ch.Name.ToString() == obj.Name.ToString()) as Character;

		internal unsafe static void Redraw(this Character obj) {
			var actor = &obj.GetStruct()->GameObject;
			actor->DisableDraw();
			actor->EnableDraw();
		}

		internal static (string,string) GetNameAndWorld(this Character chara) {
			var worldName = "";
			if (chara is PlayerCharacter pc) {
				var world = pc.HomeWorld.GameData;
				if (chara.ObjectIndex >= 200 && chara.ObjectIndex < 240) {
					var ovw = (PlayerCharacter?)chara.FindOverworldEquiv();
					if (ovw != null) world = ovw.HomeWorld.GameData;
				}

				if (world != null)
					worldName = world.Name;
			}

			return (chara.Name.ToString(), worldName);
		}
	}
}
