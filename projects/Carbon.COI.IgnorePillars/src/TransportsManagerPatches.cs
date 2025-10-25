using HarmonyLib;

namespace Carbon.COI.IgnorePillars;

[HarmonyPatch(typeof(Mafi.Core.Factory.Transports.TransportsManager))]
internal static class TransportsManagerPatches
{
    [HarmonyPrefix, HarmonyPatch("CanBuildOrJoinTransport")]
    public static void CanBuildOrJoinTransport(ref bool ignorePillars, ref bool skipExtraPillarsForBetterVisuals)
    {
        ignorePillars = true;
        skipExtraPillarsForBetterVisuals = true;
    }
}
