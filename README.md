# CompactSerializer
This repository contains compact and quick approach for serializing plain .Net objects that have to be stored in a cache. The main idea is to enumerate the same list of object properties during write and read, and save/restore their byte representations in the exactly identical sequence, one after another, without any metadata.

Though, according to MSDN, `Type.GetProperties()` method does not guaranty returning properties in the declaration or alphabetical order, the sequence returned for the same version of the same type obviously stays identical from call to call. Thus, even re-created serializer should correctly restore previously serialized object. Sure, when the set of object properties is modified, this approach will not work, but as as indicated above, the intended use is to store data in cache. The serialization process includes saving type version, and the easiest way to handle object's source code modification is just invalidate corresponding key in the cache upon the system's update. (That doesn't mean that other ways aren't possible.)


Reflection is not considered a fast mechanism, and calling `Type.GetProperties()` for each serialization and deserialization operation may not be very productive. The more effective approach is to take the properties list once and generate a code with corresponding sequence of instructions. The Reflection.Emit namespace and ILGenerator class offers suitable code generation capabilities.


The compilation of emitted code can take a while, but if the system runs for a long time, that delay will be fully compensated by the performance increase of following saving/restoring operations.


For the initial implementation, only simple property types are supported, without classes, structures, circular references etc, but including simple containers and Nullable. The exact list of supported types is:

1. `byte`
2. `bool`
3. `int`, `short`, `long`
4. `uint`, `ushort`, `ulong`
5. `double`, `float`, `decimal`
6. `Guid`
7. `DateTime`
8. `DateTimeOffset`
9. `char`
10. `string`
11. `Array&lt;T&gt;` (`T[]`), where `T` - one of the types listed above;
12. Containers, implementing `ICollection<T>` and having parameterless constructor. (`T` should be one of the types listed in points 1-10);  
13. `Nullable&lt;T&gt;`, where `T` is one of the types listed in points 1-10;  

For some of them getting bytes representation is trivial, such as calling `Guid.ToByteArray()` or `BitConverter.GetBytes(value)`, for other more complex logic had to be applied.


This implementation was compared to `System.Runtime.Serialization.Formatters.Binary.BinaryFormatter` and Newtonsoft JsonSerializer on performance speed and representation bytes count. When run with object of the kind it was designed for, even pure Reflection version performed faster, than these two serializers. In compactness, surely, both Reflection and Reflection.Emit realizations were superior to library analogs.


Benchmarking made by [BenchmarkDotNet](https://benchmarkdotnet.org)

Example of the experiment benchmarking for 1000 and 10000 iterations are like this:


|      Method |  SerializerType |     N |         Mean |        Error |       StdDev |   Gen 0 | Allocated |
|------------ |---------------- |------ |-------------:|-------------:|-------------:|--------:|----------:|
|   Serialize | BinaryFormatter | 10000 | 261,061.9 ns | 3,071.433 ns | 2,564.786 ns | 22.4609 |   35539 B |
| Deserialize | BinaryFormatter | 10000 |           NA |           NA |           NA |     N/A |       N/A |
|   Serialize |            Emit | 10000 |   4,495.2 ns |    89.016 ns |    98.941 ns |  3.0670 |    4832 B |
| Deserialize |            Emit | 10000 |   1,081.9 ns |    18.324 ns |    17.140 ns |  1.2302 |    1936 B |
|   Serialize |  NewtonsoftJson | 10000 |  30,881.8 ns |   387.899 ns |   362.841 ns |  8.2397 |   12992 B |
| Deserialize |  NewtonsoftJson | 10000 |     518.6 ns |    13.086 ns |    22.920 ns |  1.6775 |    2640 B |
|   Serialize |        Protobuf | 10000 |   7,278.8 ns |   130.767 ns |   122.320 ns |  2.1896 |    3456 B |
| Deserialize |        Protobuf | 10000 |     462.9 ns |     9.683 ns |     8.584 ns |  0.1674 |     264 B |
|   Serialize |      Reflection | 10000 |  31,657.7 ns |   597.716 ns |   559.104 ns |  4.9438 |    7805 B |
| Deserialize |      Reflection | 10000 |  27,168.5 ns |   527.481 ns |   586.293 ns |  3.8147 |    6034 B |

`Emit` serializer produced stream with size of **466** bytes
`Reflection` serializer produced stream with size of **466** bytes
`Protobuf` serializer produced stream with size of **499** bytes
`BinaryFormatter` serializer produced stream with size of **2435** bytes
`NewtonsoftJson` serializer produced stream with size of **1160** bytes