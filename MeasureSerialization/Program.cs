using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
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
            var emitSummary = BenchmarkRunner.Run<EmitSerializerBenchmark>();
            var protobufSummary = BenchmarkRunner.Run<ProtobufSerializerBenchmark>();


            //var switcher = new BenchmarkSwitcher(new[] {
            //    typeof(EmitSerializerBenchmark),
            //    typeof(ReflectionSerializerBenchmark),
            //    typeof(ProtobufSerializerBenchmark),
            //    typeof(BinarySerializerBenchmark),
            //    typeof(NewtonsoftSerializerBenchmark),
            //});
            //switcher.Run(args);
        }
    }
}
