using System;
using System.Reflection;

using Mafi.Core.Buildings.RainwaterHarvesters;
using Mafi.Core.Buildings.Shipyard;
using Mafi.Core.Buildings.VehicleDepots;
using Mafi.Core.Entities.Static;
using Mafi.Core.Factory.WellPumps;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;

namespace Carbon.COI.QuickBuildUnlocked;


public sealed class QuickBuildUnlockedMod : DataOnlyMod
{
    /// <summary>
    /// Human-readable name of the mod. This value will be showed in-game.
    /// </summary>
    public override string Name => this.GetType().Namespace.Replace('.', '-');


    /// <summary>
    /// Version of the mod, currently unused.
    /// </summary>
    public override int Version => 1;


    /// <summary>
    /// Mod constructor that lists mod dependencies as parameters.
    /// This guarantee that all listed mods will be loaded before this mod.
    /// It is a good idea to depend on both `Mafi.Core.CoreMod` and `Mafi.Base.BaseMod`.
    /// </summary>
    public QuickBuildUnlockedMod(Mafi.Core.CoreMod coreMod, Mafi.Base.BaseMod baseMod)
    {
    }


    /// <summary>
    /// Register all prototypes of this mod.
    /// </summary>
    /// <param name="registrator"></param>
    public override void RegisterPrototypes(ProtoRegistrator registrator)
    {
        RestoreQuickBuildability<WellPumpProto>(registrator.PrototypesDb);
        RestoreQuickBuildability<RainwaterHarvesterProto>(registrator.PrototypesDb);
        RestoreQuickBuildability<ShipyardProto>(registrator.PrototypesDb);
        RestoreQuickBuildability<VehicleDepotProto>(registrator.PrototypesDb);
    }


    private static void RestoreQuickBuildability<TProto>(ProtosDb protosDb) where TProto : Proto
    {
        FieldInfo protoParamsFieldInfo = typeof(Proto).GetField("m_params", BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (Proto proto in protosDb.All<TProto>())
        {
            if (proto.HasParam<DisableQuickBuildParam>() is false)
                continue;
            var protoParams = (Mafi.Collections.Dict<Type, IProtoParam>)protoParamsFieldInfo.GetValue(proto);
            protoParams.Remove(typeof(DisableQuickBuildParam));
        }
    }
}
