using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;
using static NodaTime.Serialization.Utf8Json.Tests.TestHelper;
using JsonSerializer = Utf8Json.JsonSerializer;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    /// <summary>
    /// The same tests as NodaDateIntervalConverterTest, but using the ISO-based interval converter.
    /// </summary>
    public class NodaIsoDateIntervalConverterTest
    {
        private static readonly IJsonFormatterResolver Settings = CompositeResolver.Create(new[]
        {
            NodaFormatters.IsoDateIntervalFormatter, NodaFormatters.LocalDateFormatter
        }, new[] { StandardResolver.Default });

        [Fact]
        public void RoundTrip()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);
            AssertConversions(dateInterval, "\"2012-01-02/2013-06-07\"", Settings);
        }

        [Theory]
        [InlineData("\"2012-01-022013-06-07\"")]
        public void InvalidJson(string json)
        {
            AssertInvalidJson<DateInterval>(json, Settings);
        }

        [Fact]
        public void Serialize_InObject()
        {
            var startLocalDate = new LocalDate(2012, 1, 2);
            var endLocalDate = new LocalDate(2013, 6, 7);
            var dateInterval = new DateInterval(startLocalDate, endLocalDate);

            var testObject = new TestObject { Interval = dateInterval };

            var json = JsonSerializer.ToJsonString(testObject,  Settings);

            var expectedJson = "{\"Interval\":\"2012-01-02/2013-06-07\"}";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize_InObject()
        {
            var json = "{\"Interval\":\"2012-01-02/2013-06-07\"}";

            var testObject = JsonSerializer.Deserialize<TestObject>(json, Settings);

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
