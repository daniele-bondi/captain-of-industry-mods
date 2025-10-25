using HarmonyLib;

namespace Carbon.COI.KeepMinizippers;

[HarmonyPatch(typeof(Mafi.Core.Factory.Transports.TransportsManager))]
internal static class TransportsManagerPatches
{
    private static KeepMinizippersModConfig? modConfig;


    public static void Initialize(KeepMinizippersModConfig modConfig)
    {
        TransportsManagerPatches.modConfig = modConfig;
    }


    [HarmonyPrefix, HarmonyPatch("removeZipperIfNeeded")]
    internal static bool RemoveZipperIfNeeded()
    {
        // To skip original method: return false.
        // To call original method: return true.
        bool skipOriginalMethod = TransportsManagerPatches.modConfig!.IsEnabled;
        return skipOriginalMethod is false;
    }
}
