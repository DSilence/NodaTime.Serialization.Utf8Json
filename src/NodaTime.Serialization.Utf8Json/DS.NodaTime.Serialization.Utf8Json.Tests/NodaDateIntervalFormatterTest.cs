using DS.NodaTime.Serialization.Utf8Json;
using DS.NodaTime.Serialization.Utf8Json.Enums;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;
using static NodaTime.Serialization.Utf8Json.Tests.TestHelper;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    public class NodaDateIntervalFormatterTest
    {
        private static readonly IJsonFormatterResolver Resolver = CompositeResolver.Create(new IJsonFormatter[]
        {
            NodaFormatters.CreateDateIntervalFormatter(NameHandling.Ordinal), NodaFormatters.LocalDateFormatter
        }, new[] { StandardResolver.Default });

        private static readonly IJsonFormatterResolver SettingsCamelCase = CompositeResolver.Create(new IJsonFormatter[]
        {
            NodaFormatters.CreateDateIntervalFormatter(NameHandling.CamelCase), NodaFormatters.LocalDateFormatter
        }, new[] { StandardResolver.CamelCase });

        [Fact]
        public void RoundTrip()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}", Resolver);
        }

        [Fact]
        public void RoundTrip_CamelCase()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}", SettingsCamelCase);
        }

        [Fact]
        public void Serialize_InObject()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonSerializer.ToJsonString(testObject, Resolver);

            var expectedJson = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Serialize_InObject_CamelCase()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonSerializer.ToJsonString(testObject, SettingsCamelCase);

            var expectedJson = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize_InObject()
        {
            var json = "{\"Interval\":{\"Start\":\"2012-01-02\",\"End\":\"2013-06-07\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, Resolver);

            var interval = testObject.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.Equal(expectedInterval, interval);
        }

        [Fact]
        public void Deserialize_InObject_CamelCase()
        {
            var json = "{\"interval\":{\"start\":\"2012-01-02\",\"end\":\"2013-06-07\"}}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, SettingsCamelCase);

            var interval = testObject.Interval;

            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var expectedInterval = new DateInterval(startLocalDate, endLocalDate);
            Assert.Equal(expectedInterval, interval);
        }

        public class TestObject
        {
            public DateInterval Interval { get; set; }
        }
    }
}
