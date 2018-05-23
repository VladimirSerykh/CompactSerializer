using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CompactSerializer
{
    public class BinaryFormatterSerializer<TObject> : CompactSerializerBase<TObject>
        where TObject : class, new()
    {
        private readonly BinaryFormatter _binaryFormatter = new BinaryFormatter();

        public override void Serialize(TObject theObject, Stream stream)
        {
            _binaryFormatter.Serialize(stream, theObject);
        }

        public override TObject Deserialize(Stream stream)
        {
            return (TObject)_binaryFormatter.Deserialize(stream);
        }
    }
}