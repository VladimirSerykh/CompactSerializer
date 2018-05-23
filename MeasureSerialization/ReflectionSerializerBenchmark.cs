using CompactSerializer;
using SourcesForIL;

namespace MeasureSerialization
{
    public class ReflectionSerializerBenchmark : BaseSerializerBenchmark
    {
        protected override CompactSerializerBase<Entity> CreateSerializer()
        {
            return new ReflectionCompactSerializer<Entity>();
        }
    }
}