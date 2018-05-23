using System;
using System.Collections.Generic;
using System.Text;

namespace MeasureSerialization
{
    public enum SerializerType
    {
        Emit,
        Reflection,
        Protobuf,
        BinaryFormatter,
        NewtonsoftJson,
    }
}
