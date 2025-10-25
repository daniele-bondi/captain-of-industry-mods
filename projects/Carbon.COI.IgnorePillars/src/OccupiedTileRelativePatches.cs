using HarmonyLib;

using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Terrain;

namespace Carbon.COI.IgnorePillars;

/// <summary>
/// Patches the code that makes the game require pillars when building a new entity.
/// </summary>
[HarmonyPatch(typeof(OccupiedTileRelative))]
internal static class OccupiedTileRelativePatches
{
    /// <summary>
    /// <see cref="OccupiedTileRelative(short, short, short, ushort, ushort, TileSurfaceSlimId, short)"/>
    /// </summary>
    /// <remarks>
    /// This patch does not use <c>__instance</c> because <see cref="OccupiedTileRelative.ConstraintSlim"/> is <see langword="readonly"/>.
    /// </remarks>
    [HarmonyPatch(MethodType.Constructor, typeof(short), typeof(short), typeof(short), typeof(ushort), typeof(ushort), typeof(TileSurfaceSlimId), typeof(short))]
    public static void RemovePillarConstraint(ref ushort ___ConstraintSlim)
    {
        var constraintWithoutPillar = ((LayoutTileConstraint)___ConstraintSlim) & ~LayoutTileConstraint.UsingPillar;
        ___ConstraintSlim = (ushort)constraintWithoutPillar;
    }


    /// <summary>
    /// <see cref="OccupiedTileRelative.Constraint"/>
    /// </summary>
    [HarmonyPostfix, HarmonyPatch("Constraint", MethodType.Getter)]
    public static void IgnorePillarConstraint(ref LayoutTileConstraint __result)
    {
        __result &= ~LayoutTileConstraint.UsingPillar;
    }
}
