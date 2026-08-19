using HarmonyLib;
using Multiplayer.API;
using RimWorld;
using RimWorld.Planet;

namespace StkMultiplayerPatch;

// This affects ritual outcome "NearbyFactionGoodwill"
// We should exclude other player factions from it, because it does nothing
// With this applied these rituals will target correct npc factions instead
[HarmonyPatch(typeof(Faction), nameof(Faction.CanChangeGoodwillFor))]
public static class Patch_Faction_CanChangeGoodwillFor
{
	[HarmonyPostfix]
	public static void Postfix(Faction __instance, Faction other, int goodwillChange, ref bool __result)
	{
		if (!MP.IsInMultiplayer || !__result)
			return;

		if (__instance.IsPlayer && other.IsPlayer)
			__result = false;
	}

}

// This affects "TradeRequest" quests. Despite how funny it was to see other
// player requesting 50 kids pants, AND IT ACTUALLY WORKS, I doubt it is intended,
// and it consumes a quest "slot" that would be otherwise generated for a correct
// faction. "Visitable" already checks if the settlement is "OfPlayer" factions,
// but we also want to check that it is not "IsPlayer", for multifaction. This
// method is used in quest generation checks and in caravan gizmos. Just in case
// you wonder, it still allows your pawns to get into other players settlements.
[HarmonyPatch(typeof(Settlement), nameof(Settlement.Visitable), MethodType.Getter)]
public static class Patch_Settlement_Visitable
{
	[HarmonyPostfix]
	public static void Postfix(Settlement __instance, ref bool __result)
	{
		if (!MP.IsInMultiplayer || !__result)
			return;

		if (__instance.Faction.IsPlayer)
			__result = false;
	}

}
