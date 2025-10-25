using System.Linq;
using System.Reflection;

using Mafi;
using Mafi.Core.Entities.Static.Layout;
using Mafi.Core.Factory.Lifts;
using Mafi.Core.Factory.Sorters;
using Mafi.Core.Factory.Zippers;
using Mafi.Core.Prototypes;

namespace Carbon.COI.IgnorePillars;


/// <summary>
/// Uses a <see cref="ProtosDb"/> to remove <see cref="LayoutTileConstraint.UsingPillar"/> from prototypes.
/// </summary>
/// <remarks>
/// We use a dedicated class marked with <see cref="GlobalDependencyAttribute"/> because, according to the doc-comment on <see cref="Mafi.Core.Mods.IMod"/>,
/// global dependencies are instantiated only after ALL mods have completed their <see cref="Mafi.Core.Mods.IMod.RegisterPrototypes(Mafi.Core.Mods.ProtoRegistrator)"/> stage.
/// <br/>
/// We want to execute after all protos are registered because we want to modify protos added by other mods without having to declare an explicit dependency on them.
/// <br/>
/// Declaring an explicit dependency is inconvenient because the dependency list would be static and would require manual updating to support new mods.
/// In addition, as of the time of writing, it seems like the mechanism to declare optional mod dependencies is broken, so the static list would make dependencies mandatory.
/// </remarks>
[GlobalDependency(RegistrationMode.AsEverything, onlyInDebug: false, onlyInDevOnly: false)]
public sealed class PillarConstraintRemover
{
    public PillarConstraintRemover(ProtosDb protosDb)
    {
        RemovePillarConstraint<MiniZipperProto>(protosDb);
        RemovePillarConstraint<ZipperProto>(protosDb);
        RemovePillarConstraint<SorterProto>(protosDb);
        RemovePillarConstraint<LiftProto>(protosDb);
    }


    /// <summary>
    /// Change all the prototypes of a given class so that they won't collapse if they are on unstable terrain, which includes not having a pillar underneath.
    /// </summary>
    private static void RemovePillarConstraint<TProto>(ProtosDb protosDb) where TProto : LayoutEntityProto
    {
        FieldInfo layoutContraintFieldInfo = typeof(EntityLayout).GetField("CombinedConstraint", BindingFlags.Instance | BindingFlags.Public);

        foreach (var layout in protosDb.All<TProto>().Select(proto => proto.Layout))
        {
            LayoutTileConstraint constraintWithoutPillar = layout.CombinedConstraint & ~LayoutTileConstraint.UsingPillar;
            layoutContraintFieldInfo.SetValue(layout, constraintWithoutPillar);
        }
    }
}
