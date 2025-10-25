using Mafi;
using Mafi.Core.Mods;
using Mafi.Serialization;

namespace Carbon.COI.KeepMinizippers;

[GlobalDependency(RegistrationMode.AsEverything, onlyInDebug: false, onlyInDevOnly: false)]
[GenerateSerializer(customClassDataSerialization: false, serializeAsSingleton: null, newInVersion: 0)]
public class KeepMinizippersModConfig : IModConfig
{
    public static readonly int ConfigVersion = 1;

    public bool IsEnabled { get; set; }


    #region Serialization stuff

    public static void Serialize(KeepMinizippersModConfig value, BlobWriter writer)
    {
        if (writer.TryStartClassSerialization(value) is false)
            return;

        static void SerializeAction(object value, BlobWriter writer) => ActualSerialize((KeepMinizippersModConfig)value, writer);
        writer.EnqueueDataSerialization(value, SerializeAction);
    }


    private static void ActualSerialize(KeepMinizippersModConfig modConfig, BlobWriter writer)
    {
        writer.WriteInt(ConfigVersion);
        writer.WriteBool(modConfig.IsEnabled);
    }


    public static KeepMinizippersModConfig Deserialize(BlobReader reader)
    {
        if (reader.TryStartClassDeserialization(out KeepMinizippersModConfig modConfig) is false)
            return modConfig;

        static void DeserializeAction(object value, BlobReader reader) => ActualDeserialize((KeepMinizippersModConfig)value, reader);
        reader.EnqueueDataDeserialization(modConfig, DeserializeAction, parent: null);
        return modConfig;
    }


    private static void ActualDeserialize(KeepMinizippersModConfig modConfig, BlobReader reader)
    {
        int configVersion = reader.ReadInt();
        modConfig.IsEnabled = reader.ReadBool();
    }

    #endregion Serialization stuff
}
