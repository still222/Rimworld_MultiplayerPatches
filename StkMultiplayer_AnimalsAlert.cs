using System.Collections.Generic;
using HarmonyLib;
using Multiplayer.API;
using RimWorld;
using Verse;

namespace StkMultiplayerPatch;

// Vanilla uses (mostly) "needs.food.TicksStarving" to determine
// if animals are hungry/starving. Which in async results in
// constant warnings, when viewed from another, higher tick map.
// We ensure that animals in the list are actually starving.
[HarmonyPatch(typeof(Alert_StarvationAnimals), nameof(Alert_StarvationAnimals.StarvingAnimals), MethodType.Getter)]
public static class Patch_Alert_StarvationAnimals
{
	[HarmonyPostfix]
	static void Postfix(Alert_StarvationAnimals __instance, ref List<Pawn> __result)
	{
		if (!MP.IsInMultiplayer)
			return;

		for (int i = __instance.starvingAnimalsResult.Count - 1; i >= 0; i--)
		{
			Pawn animal = __instance.starvingAnimalsResult[i];

			if (!animal.needs.food.Starving)
				__instance.starvingAnimalsResult.RemoveAt(i);
		}
		
		__result = __instance.starvingAnimalsResult;
	}

}

// Exact same idea as in "Patch_Alert_StarvationAnimals",
// but this class is written slightly differently
[HarmonyPatch(typeof(Alert_PennedAnimalHungry), nameof(Alert_PennedAnimalHungry.CalculateTargets))]
public static class Patch_Alert_PennedAnimalHungry
{
	[HarmonyPostfix]
	static void Postfix(Alert_PennedAnimalHungry __instance)
	{
		if (!MP.IsInMultiplayer)
			return;

		for (int i = __instance.targets.Count - 1; i >= 0; i--)
		{
			Pawn animal = __instance.targets[i].Pawn;

			if (!animal.needs.food.Starving)
			{
				__instance.targets.RemoveAt(i);
				__instance.pawnNames.RemoveAt(i);
			}

		}

	}

}
