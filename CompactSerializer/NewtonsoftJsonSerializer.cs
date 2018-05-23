using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace CompactSerializer
{
    public class NewtonsoftJsonSerializer<TObject> : CompactSerializerBase<TObject>
        where TObject : class, new()
    {
        public override void Serialize(TObject theObject, Stream stream)
        {
            var jsonStr = JsonConvert.SerializeObject(theObject);
            WriteString(stream, jsonStr);
        }

        public override TObject Deserialize(Stream stream)
        {
            var jsonStr = ReadString(stream);
            return JsonConvert.DeserializeObject<TObject>(jsonStr);
        }
    }
}