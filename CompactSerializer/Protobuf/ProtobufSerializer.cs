using ProtoBuf;
using System.IO;

namespace CompactSerializer.Protobuf
{
    public class ProtobufSerializer<TObject> : CompactSerializerBase<TObject>
        where TObject : class, new()
    {
        public override TObject Deserialize(Stream stream)
        {
            return (TObject)Serializer.NonGeneric.Deserialize(typeof(TObject), stream);
        }

        public override void Serialize(TObject theObject, Stream stream)
        {
            Serializer.NonGeneric.Serialize(stream, theObject);
        }
    }
}
