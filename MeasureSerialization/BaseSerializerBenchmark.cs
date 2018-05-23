using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using CompactSerializer;
using SourcesForIL;

namespace MeasureSerialization
{
    [CoreJob]
    [OrderProvider(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn(NumeralSystem.Roman)]
    [MemoryDiagnoser]
    public abstract class BaseSerializerBenchmark
    {
        private Entity _entity;

        private readonly MemoryStream _stream = new MemoryStream();

        private readonly MemoryStream _serializedStream = new MemoryStream();

        [Params(/*100, */1000/*, 10000, 100000*/)]
        public int N;

        protected CompactSerializerBase<Entity> _serializer;

        [GlobalSetup]
        public void Setup()
        {
            _entity = new Entity
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

            _serializer = CreateSerializer();

            _serializer.Serialize(_entity, _serializedStream);
        }

        protected abstract CompactSerializerBase<Entity> CreateSerializer();

        [Benchmark]
        public void Serialize() => _serializer.Serialize(_entity, new MemoryStream());


        [Benchmark]
        public Entity Deserialize() => _serializer.Deserialize(_serializedStream);
    }
}