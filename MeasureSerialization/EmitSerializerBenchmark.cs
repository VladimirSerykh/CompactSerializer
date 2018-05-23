using CompactSerializer;
using CompactSerializer.GeneratedSerializer;
using SourcesForIL;

namespace MeasureSerialization
{
    public class EmitSerializerBenchmark : BaseSerializerBenchmark
    {
        protected override CompactSerializerBase<Entity> CreateSerializer()
        {
            return EmitSerializerGenerator.Generate<Entity>();
        }
    }
}