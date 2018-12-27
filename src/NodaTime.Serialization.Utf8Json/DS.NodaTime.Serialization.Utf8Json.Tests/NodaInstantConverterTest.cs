using System;
using Utf8Json;
using Utf8Json.Formatters;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    public class NodaInstantConverterTest
    {
        private static readonly IJsonFormatterResolver Settings = CompositeResolver.Create(new IJsonFormatter[]
        {
            NodaFormatters.InstantFormatter,
            NodaFormatters.NullableInstantFormatter
        }, new[] { StandardResolver.Default });

        [Fact]
        public void Serialize_NonNullableType()
        {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonSerializer.ToJsonString(instant, Settings);
            var expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Serialize_NullableType_NonNullValue()
        {
            Instant? instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonSerializer.ToJsonString(instant, Settings);
            var expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Serialize_NullableType_NullValue()
        {
            Instant? instant = null;
            var json = JsonSerializer.ToJsonString(instant, Settings);
            var expectedJson = "null";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize_ToNonNullableType()
        {
            var json = "\"2012-01-02T03:04:05Z\"";
            var instant = JsonSerializer.Deserialize<Instant>(json, Settings);
            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.Equal(expectedInstant, instant);
        }

        [Fact]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            var json = "\"2012-01-02T03:04:05Z\"";
            var instant = JsonSerializer.Deserialize<Instant?>(json, Settings);
            Instant? expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.Equal(expectedInstant, instant);
        }

        [Fact]
        public void Deserialize_ToNullableType_NullValue()
        {
            var json = "null";
            var instant = JsonSerializer.Deserialize<Instant?>(json, Settings);
            Assert.Null(instant);
        }
        
        [Fact]
        public void Serialize_EquivalentToIsoDateTimeConverter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(dateTime);
            var jsonDateTime = JsonSerializer.ToJsonString(dateTime, CompositeResolver.Create(new ISO8601DateTimeFormatter()));
            var jsonInstant = JsonSerializer.ToJsonString(instant, Settings);
            Assert.Equal(jsonDateTime, jsonInstant);
        }
    }
}
