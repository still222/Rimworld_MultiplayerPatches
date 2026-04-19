using HarmonyLib;
using Multiplayer.API;
using RimWorld;
using RimWorld.Planet;
using Verse;


namespace StkMultiplayerPatch;

[HarmonyPatch(typeof(SettlementDefeatUtility), nameof(SettlementDefeatUtility.IsDefeated), typeof(Map), typeof(Faction))]
public static class Patch_SettlementDefeatUtility_IsDefeated
{
	static bool Prefix(Faction faction, ref bool __result)
	{
		// We run original if's not MP, or faction owning the map is not any player
		if (!MP.IsInMultiplayer || !faction.IsPlayer)
			return true;

		// We skip checking if "enemy" is defeated on any player base (beacuse other players could be friendly)
		__result = false;
		return false;
	}
}

[HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.TryExecute))]
public static class Patch_IncidentWorker_TryExecute_ForMultifactionTriggering
{
	static bool Prefix(IncidentParms parms, ref bool __result)
	{
		// We make some sanity checks and assume, that empty (without any colonists)
		// factionless maps (such as dungeones) are impossible and instantly abandoned.
		if (!MP.IsInMultiplayer ||
			parms.target is not Map map ||
			map.ParentFaction == Faction.OfPlayer ||
			map.mapPawns.AnyColonistSpawned)
		{
			Log.Message($"[StkMPPatch] Incident greenlit");
			return true;
		}

		// Skip incidents if we couldn't do it
		Log.Warning($"[StkMPPatch] Incident shutdown on a {map}");
		__result = true;
		return false;
	}
}

[HarmonyPatch(typeof(LetterStack), nameof(LetterStack.ReceiveLetter), typeof(Letter), typeof(string), typeof(int), typeof(bool))]
static class LetterStackReceiveFactionDebug
{
	// todo the letter might get culled from the archive if it isn't in the stack and Sync depends on the archive
	static void Prefix()
	{
		Log.Message($"[StkMPPatch] Current Incident Faction: {Faction.OfPlayer}");
	}
}