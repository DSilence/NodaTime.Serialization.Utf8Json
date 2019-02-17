using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using DS.NodaTime.Serialization.Utf8Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Utf8Json;
using Utf8Json.Resolvers;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class OffsetDateTimeBenchmark
    {
        private readonly OffsetDateTime _offsetDateTime = new OffsetDateTime(new LocalDateTime(2012, 10, 14, 15, 30), Offset.FromHours(3));
        private DateTimeOffset _dateTimeOffset;
        private MemoryStream _memoryStream;
        private StreamWriter _streamWriter;
        private JsonTextWriter _textWriter;
        private IJsonFormatterResolver _resolver;
        private JsonSerializer _jsonSerializer;

        [GlobalSetup]
        public void Init()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            _dateTimeOffset = _offsetDateTime.ToDateTimeOffset();
            _jsonSerializer = JsonSerializer.CreateDefault();

            _resolver = CompositeResolver.Create(new NodaTimeResolver(DateTimeZoneProviders.Tzdb),
                StandardResolver.Default);
        }

        [IterationSetup]
        public void IterationInit()
        {
            _memoryStream = new MemoryStream();
            _streamWriter = new StreamWriter(_memoryStream);
            _textWriter = new JsonTextWriter(_streamWriter);
        }

        [Benchmark]
        public ArraySegment<byte> JsonNetOffsetDateTime()
        {
            _jsonSerializer.Serialize(_textWriter, _offsetDateTime);
            _memoryStream.TryGetBuffer(out var result);
            return result;
        }

        [Benchmark]
        public ArraySegment<byte> Utf8JsonDateTimeOffset()
        {
            return global::Utf8Json.JsonSerializer.SerializeUnsafe(_dateTimeOffset);
        }

        [Benchmark]
        public object Utf8JsonOffsetDateTime()
        {
            return global::Utf8Json.JsonSerializer.SerializeUnsafe(_offsetDateTime, _resolver);
        }
    }
}
