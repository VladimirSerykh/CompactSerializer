using CompactSerializer;
using SourcesForIL;

namespace MeasureSerialization
{
    public class NewtonsoftSerializerBenchmark : BaseSerializerBenchmark
    {
        protected override CompactSerializerBase<Entity> CreateSerializer()
        {
            return new NewtonsoftJsonSerializer<Entity>();
        }
    }
}