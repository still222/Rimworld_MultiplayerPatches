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
		var harmony = new Harmony("stk.sfw.patcher");
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
			MP.RegisterAll();
		}
	}

}