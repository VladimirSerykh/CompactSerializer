using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using CompactSerializer;
using CompactSerializer.GeneratedSerializer;
using CompactSerializer.Protobuf;
using ProtoBuf;
using ProtoBuf.Meta;
using SourcesForIL;

namespace MeasureSerialization
{
    [CoreJob]
    [MemoryDiagnoser]
    [RankColumn]
    public class SerializerBenchmark
    {
        public Entity Entity = new Entity
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
            Tags = new List<string> { "The quick brown fox jumps over the lazy dog", "Reflection.Emit", string.Empty, "0" },
            AlternativeId = Guid.NewGuid()
        };

        private CompactSerializerBase<Entity> _serializer;

        private readonly MemoryStream _stream = new MemoryStream();

        private readonly MemoryStream _serializedStream = new MemoryStream();

        [Params(SerializerType.Emit, SerializerType.Reflection, SerializerType.Protobuf, SerializerType.BinaryFormatter, SerializerType.NewtonsoftJson)]
        public SerializerType SerializerType;

        [Params(/*10, 1000, */10000/*, 100000*/)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _serializer = CreateSerializer(SerializerType);

            _serializer.Serialize(Entity, _serializedStream);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _serializedStream.Seek(0, SeekOrigin.Begin);
        }

        [Benchmark]
        public void Serialize() => _serializer.Serialize(Entity, new MemoryStream());
        
        [Benchmark]
        public Entity Deserialize() => _serializer.Deserialize(_serializedStream);

        public CompactSerializerBase<Entity> CreateSerializer(SerializerType serializerType)
        {
            switch (serializerType)
            {
                case SerializerType.Emit:
                    return CreateEmitSerializer();
                case SerializerType.Reflection:
                    return CreateReflectionSerializer();
                case SerializerType.Protobuf:
                    return CreateProtobufSerializer();
                case SerializerType.BinaryFormatter:
                    return CreateBinaryFormatterSerializer();
                case SerializerType.NewtonsoftJson:
                    return CreateNewtonsoftJsonSerializer();
                default:
                    throw new ArgumentOutOfRangeException(nameof(serializerType));
            }
        }

        private EmitSerializer<Entity> CreateEmitSerializer()
        {
            return EmitSerializerGenerator.Generate<Entity>();
        }

        private CompactSerializerBase<Entity> CreateReflectionSerializer()
        {
            return new ReflectionCompactSerializer<Entity>();
        }

        private CompactSerializerBase<Entity> CreateProtobufSerializer()
        {
            // Register DateTimeOffsetSurrogate (because protobuf-net doesn't support DateTimeOffset)
            RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
            // Precompile serializer for Entity
            Serializer.PrepareSerializer<Entity>();

            return new ProtobufSerializer<Entity>();
        }

        private CompactSerializerBase<Entity> CreateBinaryFormatterSerializer()
        {
            return new BinaryFormatterSerializer<Entity>();
        }

        private CompactSerializerBase<Entity> CreateNewtonsoftJsonSerializer()
        {
            return new NewtonsoftJsonSerializer<Entity>();
        }
    }
}