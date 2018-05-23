using CompactSerializer;
using SourcesForIL;

namespace MeasureSerialization
{
    public class BinarySerializerBenchmark : BaseSerializerBenchmark
    {
        protected override CompactSerializerBase<Entity> CreateSerializer()
        {
            return new BinaryFormatterSerializer<Entity>();
        }
    }
}