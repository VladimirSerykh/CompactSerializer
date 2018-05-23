using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Linq;

namespace MeasureSerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var emitSummary = BenchmarkRunner.Run<SerializerBenchmark>();

            Console.WriteLine();

            var benchmark = new SerializerBenchmark();

            new[]
                {
                    SerializerType.Emit,
                    SerializerType.Reflection,
                    SerializerType.Protobuf,
                    SerializerType.BinaryFormatter,
                    SerializerType.NewtonsoftJson,
                }
                .Select(t => new
                    {
                        Serialier = benchmark.CreateSerializer(t),
                        Type = t,
                    })
                .Select(v =>
                {
                    var stream = new MemoryStream();
                    v.Serialier.Serialize(benchmark.Entity, stream);
                    return new
                    {
                        Stream = stream,
                        Type = v.Type,
                    };
                })
                .Select(v =>
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(v.Type);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" serializer produced stream with size of ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(v.Stream.Length);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" bytes");
                    Console.WriteLine();
                    return v;
                })
                .ToArray();
        }
    }
}
