using NodaTime.TimeZones;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    public class NodaDateTimeZoneFormatterTest
    {
        private static readonly IJsonFormatterResolver FormatterResolver = CompositeResolver.Create(NodaFormatters.CreateDateTimeZoneFormatter(DateTimeZoneProviders.Tzdb));

        [Fact]
        public void Serialize()
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var json = JsonSerializer.ToJsonString(dateTimeZone, FormatterResolver);
            var expectedJson = "\"America/Los_Angeles\"";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize()
        {
            var json = "\"America/Los_Angeles\"";
            var dateTimeZone = JsonSerializer.Deserialize<DateTimeZone>(json, FormatterResolver);
            var expectedDateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            Assert.Equal(expectedDateTimeZone, dateTimeZone);
        }

        [Fact]
        public void Deserialize_TimeZoneNotFound()
        {
            var json = "\"America/DOES_NOT_EXIST\"";
            Assert.Throws<DateTimeZoneNotFoundException>(() => JsonSerializer.Deserialize<DateTimeZone>(json, FormatterResolver));
        }
    }
}
