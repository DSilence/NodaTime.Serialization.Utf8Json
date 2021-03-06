using DS.NodaTime.Serialization.Utf8Json;
using DS.NodaTime.Serialization.Utf8Json.Enums;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    public class NodaIntervalConverterTest
    {
        private static readonly IJsonFormatterResolver Resolver = CompositeResolver.Create(new IJsonFormatter[]
        {
            NodaFormatters.CreateIntervalFormatter(NameHandling.Ordinal),
            NodaFormatters.NullableIntervalFormatter,
            NodaFormatters.InstantFormatter,
            NodaFormatters.NullableInstantFormatter
        }, new [] {StandardResolver.Default});

        private static readonly IJsonFormatterResolver ResolverCamelCase = CompositeResolver.Create(new IJsonFormatter[]
        {
            NodaFormatters.CreateIntervalFormatter(NameHandling.CamelCase),
            NodaFormatters.NullableIntervalFormatter,
            NodaFormatters.InstantFormatter,
            NodaFormatters.NullableInstantFormatter
        }, new [] {StandardResolver.CamelCase});

        [Fact]
        public void RoundTrip()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(670);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromNanoseconds(123456789);
            var interval = new Interval(startInstant, endInstant);
            TestHelper.AssertConversions(interval, "{\"Start\":\"2012-01-02T03:04:05.67Z\",\"End\":\"2013-06-07T08:09:10.123456789Z\"}", Resolver);
        }

        [Fact]
        public void RoundTrip_Infinite()
        {
            var instant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromNanoseconds(123456789);
            TestHelper.AssertConversions(new Interval(null, instant), "{\"End\":\"2013-06-07T08:09:10.123456789Z\"}", Resolver);
            TestHelper.AssertConversions(new Interval(instant, null), "{\"Start\":\"2013-06-07T08:09:10.123456789Z\"}", Resolver);
            TestHelper.AssertConversions(new Interval(null, null), "{}", Resolver);
        }

        [Fact]
        public void Serialize_InObject()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var testObject = new TestObject { Interval = interval };

            var json = JsonSerializer.ToJsonString(testObject, Resolver);

            var expectedJson = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Serialize_InObject_CamelCase()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var testObject = new TestObject { Interval = interval };

            var json = JsonSerializer.ToJsonString(testObject,  ResolverCamelCase);

            var expectedJson = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize_InObject()
        {
            var json = "{\"Interval\":{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, Resolver);

            var interval = testObject.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.Equal(expectedInterval, interval);
        }

        [Fact]
        public void Deserialize_InObject_CamelCase()
        {
            var json = "{\"interval\":{\"start\":\"2012-01-02T03:04:05Z\",\"end\":\"2013-06-07T08:09:10Z\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, ResolverCamelCase);

            var interval = testObject.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.Equal(expectedInterval, interval);
        }

        public class TestObject
        {
            public Interval Interval { get; set; }
        }
    }
}
