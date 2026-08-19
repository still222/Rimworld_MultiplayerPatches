using HarmonyLib;
using Multiplayer.API;
using RimWorld;
using Verse;

namespace StkMultiplayerPatch;

[StaticConstructorOnStartup]
public static class Startup
{
	static Startup()
	{
		var harmony = new Harmony("stk.mp.patcher");
		harmony.PatchAll();
		harmony.Unpatch(
			AccessTools.Method(typeof(SituationalThoughtHandler), "CheckRecalculateSocialThoughts"),
			HarmonyPatchType.Prefix,
			"multiplayer"
		);
		harmony.Unpatch(
			AccessTools.Method(typeof(SituationalThoughtHandler), "AppendSocialThoughts"),
			HarmonyPatchType.Transpiler,
			"multiplayer"
		);

		if (MP.enabled)
		{
			MP.RegisterSyncMethod(typeof(GameComponent_PsychicRitualManager), nameof(GameComponent_PsychicRitualManager.ClearCooldown));
		}

	}

}
