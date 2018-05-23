using System;
using CompactSerializer;
using CompactSerializer.Protobuf;
using ProtoBuf;
using ProtoBuf.Meta;
using SourcesForIL;

namespace MeasureSerialization
{
    public class ProtobufSerializerBenchmark : BaseSerializerBenchmark
    {
        protected override CompactSerializerBase<Entity> CreateSerializer()
        {
            // Register DateTimeOffsetSurrogate (because protobuf-net doesn't support DateTimeOffset)
            RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
            // Precompile serializer for Entity
            Serializer.PrepareSerializer<Entity>();

            return new ProtobufSerializer<Entity>();
        }
    }
}