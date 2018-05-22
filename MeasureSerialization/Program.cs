using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using CompactSerializer;
using CompactSerializer.GeneratedSerializer;
using CompactSerializer.Protobuf;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Meta;
using SourcesForIL;

namespace MeasureSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var originalEntity = new Entity
            {
                Name = "Name",
                ShortName = string.Empty,
                Description = null,
                Label = 'L',
                Age = 32,
                Index = -7,
                IsVisible = true,
                Price = 225.87M,
                Rating = 4.8,
                Weigth = 130,
                ShortIndex = short.MaxValue,
                LongIndex = long.MinValue,
                UnsignedIndex = uint.MaxValue,
                ShortUnsignedIndex = 25,
                LongUnsignedIndex = 11,
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                CreatedAtUtc = DateTime.UtcNow,
                LastAccessed = DateTime.MinValue,
                ChangedAt = DateTimeOffset.Now,
                ChangedAtUtc = DateTimeOffset.UtcNow,
                References = null,
                Weeks = new List<short> { 3, 12, 24, 48, 53, 61 },
                PricesHistory = new[] { 225.8M, 226M, 227.87M, 224.87M },
                BitMap = new[] { true, true, false, true, false, false, true, true },
                ChildrenIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() },
                Schedule = new[] { DateTime.Now.AddDays(-1), DateTime.Now.AddMonths(2), DateTime.Now.AddYears(10) },
                Moments = new[] { DateTimeOffset.UtcNow.AddDays(-5), DateTimeOffset.Now.AddDays(10) },
                Tags = new List<string> {"The quick brown fox jumps over the lazy dog", "Reflection.Emit", string.Empty, "0" },
                AlternativeId = Guid.NewGuid()
            };

            var repeatCount = 10000;

            Entity deserialized = null;
            var compilationStopWatch = new Stopwatch();
            compilationStopWatch.Start();
            var emitSerializer = EmitSerializerGenerator.Generate<Entity>();
            compilationStopWatch.Stop();
            Console.WriteLine("EmitSerializer compiled in: {0} ms", compilationStopWatch.Elapsed.TotalMilliseconds);

            var emitSerializerStopWatch = new Stopwatch();
            var emitSerializedBytesCount = 0L;
            var typeVersion = emitSerializer.GetTypeVersion();
            using (var stream = new MemoryStream())
            {
                for (var repeat = 0; repeat < repeatCount; repeat++)
                {
                    emitSerializerStopWatch.Start();
                    emitSerializer.WriteVersion(stream, typeVersion);
                    emitSerializer.Serialize(originalEntity, stream);
                    emitSerializerStopWatch.Stop();
                    emitSerializedBytesCount = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);
                    emitSerializerStopWatch.Start();
                    emitSerializer.ReadObjectVersion(stream);
                    deserialized = emitSerializer.Deserialize(stream);
                    emitSerializerStopWatch.Stop();
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            var reflectionBytesCount = 0L;
            var reflectionStopwatch = new Stopwatch();
            using (var stream = new MemoryStream())
            {
                var reflectionSerializer = new ReflectionCompactSerializer<Entity>();
                typeVersion = reflectionSerializer.GetTypeVersion();

                for (var repeat = 0; repeat < repeatCount; repeat++)
                {
                    reflectionStopwatch.Start();
                    reflectionSerializer.WriteVersion(stream, typeVersion);
                    reflectionSerializer.Serialize(originalEntity, stream);
                    reflectionStopwatch.Stop();
                    reflectionBytesCount = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);
                    reflectionStopwatch.Start();
                    reflectionSerializer.ReadObjectVersion(stream);
                    var deserializedEntity = reflectionSerializer.Deserialize(stream);
                    reflectionStopwatch.Stop();
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            var protobufBytesCount = 0L;
            var protobufStopwatch = new Stopwatch();
            using (var stream = new MemoryStream())
            {
                var protobufSerializer = new ProtobufSerializer<Entity>();
                // Register DateTimeOffsetSurrogate (because protobuf-net doesn't support DateTimeOffset)
                RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
                // Precompile serializer for Entity
                Serializer.PrepareSerializer<Entity>();
                typeVersion = protobufSerializer.GetTypeVersion();

                for (var repeat = 0; repeat < repeatCount; repeat++)
                {
                    protobufStopwatch.Start();
                    protobufSerializer.Serialize(originalEntity, stream);
                    protobufStopwatch.Stop();
                    protobufBytesCount = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);
                    protobufStopwatch.Start();
                    var deserializedEntity = protobufSerializer.Deserialize(stream);
                    protobufStopwatch.Stop();
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            var binaryStopWatch = new Stopwatch();
            var binaryBytesCount = 0L;
            using (var stream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
                for (var repeat = 0; repeat < repeatCount; repeat++)
                {
                    binaryStopWatch.Start();
                    binaryFormatter.Serialize(stream, originalEntity);
                    binaryStopWatch.Stop();
                    binaryBytesCount = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);
                    binaryStopWatch.Start();
                    var result = (Entity)binaryFormatter.Deserialize(stream);
                    binaryStopWatch.Stop();
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }

            var jsonStopWatch = new Stopwatch();
            var serialized = string.Empty;
            jsonStopWatch.Start();
            for (var repeat = 0; repeat < repeatCount; repeat++)
            {
                serialized = JsonConvert.SerializeObject(originalEntity);
                var jsonResult = JsonConvert.DeserializeObject<Entity>(serialized);
            }
            jsonStopWatch.Stop();
            var jsonBytesCount = Encoding.UTF8.GetBytes(serialized).Length;

            Console.WriteLine();
            Console.WriteLine("Serializer                | Average elapsed, ms | Size, bytes");
            var rowFormat = "{0,-26}| {1, -20}| {2}";
            var hor = new string('-', 61);
            Console.WriteLine(hor);

            Console.WriteLine(
                rowFormat, 
                "EmitSerializer",
                TimeSpan.FromTicks(emitSerializerStopWatch.ElapsedTicks / repeatCount).TotalMilliseconds,
                emitSerializedBytesCount);
            Console.WriteLine(hor);

            Console.WriteLine(
                rowFormat,
                "ReflectionSerializer",
                TimeSpan.FromTicks(reflectionStopwatch.ElapsedTicks / repeatCount).TotalMilliseconds,
                reflectionBytesCount);
            Console.WriteLine(hor);

            Console.WriteLine(
                rowFormat,
                "ProtobufSerializer",
                TimeSpan.FromTicks(protobufStopwatch.ElapsedTicks / repeatCount).TotalMilliseconds,
                protobufBytesCount);
            Console.WriteLine(hor);

            Console.WriteLine(
                rowFormat,
                "BinaryFormatter",
                TimeSpan.FromTicks(binaryStopWatch.ElapsedTicks / repeatCount).TotalMilliseconds,
                binaryBytesCount);
            Console.WriteLine(hor);

            Console.WriteLine(
                rowFormat,
                "Newtonsoft JsonSerializer",
                TimeSpan.FromTicks(jsonStopWatch.ElapsedTicks / repeatCount).TotalMilliseconds,
                jsonBytesCount);
            Console.WriteLine(hor);
        }
    }
}
