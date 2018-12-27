using System;
using Utf8Json;
using Utf8Json.Formatters;
using Utf8Json.Resolvers;
using Xunit;
using static NodaTime.Serialization.Utf8Json.Tests.TestHelper;

namespace NodaTime.Serialization.Utf8Json.Tests
{
     /// <summary>
    /// Tests for the formatters exposed in NodaFormatters.
    /// </summary>
    public class NodaFormattersTest
    {
        [Fact]
        public void OffsetFormatter()
        {
            var value = Offset.FromHoursAndMinutes(5, 30);
            var json = "\"+05:30\"";
            AssertConversions(value, json, NodaFormatters.OffsetFormatter);
        }

        [Fact]
        public void InstantFormatter()
        {
            var value = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = "\"2012-01-02T03:04:05Z\"";
            AssertConversions(value, json, NodaFormatters.InstantFormatter);
        }

        [Fact]
        public void InstantFormatter_EquivalentToIsoDateTimeFormatter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(dateTime);
            var jsonDateTime = JsonSerializer.ToJsonString(dateTime, CompositeResolver.Create(new ISO8601DateTimeFormatter()));
            var jsonInstant = JsonSerializer.ToJsonString(instant, CompositeResolver.Create(NodaFormatters.InstantFormatter));
            Assert.Equal(jsonDateTime, jsonInstant);
        }

        [Fact]
        public void LocalDateFormatter()
        {
            var value = new LocalDate(2012, 1, 2, CalendarSystem.Iso);
            var json = "\"2012-01-02\"";
            AssertConversions(value, json, NodaFormatters.LocalDateFormatter);
        }

        [Fact]
        public void LocalDateFormatter_SerializeNonIso_Throws()
        {
            var localDate = new LocalDate(2012, 1, 2, CalendarSystem.Coptic);

            Assert.Throws<ArgumentException>(() => JsonSerializer.ToJsonString(localDate, CompositeResolver.Create(NodaFormatters.LocalDateFormatter)));
        }

        [Fact]
        public void LocalDateTimeFormatter()
        {
            var value = new LocalDateTime(2012, 1, 2, 3, 4, 5, CalendarSystem.Iso).PlusNanoseconds(123456789);
            var json = "\"2012-01-02T03:04:05.123456789\"";
            AssertConversions(value, json, NodaFormatters.LocalDateTimeFormatter);
        }

        [Fact(Skip = "Default UTF8Json formatter leaves wierd trailing 0.")]
        public void LocalDateTimeFormatter_EquivalentToIsoDateTimeFormatter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, 6, DateTimeKind.Unspecified);
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, CalendarSystem.Iso);

            var jsonDateTime = JsonSerializer.ToJsonString(dateTime, CompositeResolver.Create(new ISO8601DateTimeFormatter()));
            var jsonLocalDateTime = JsonSerializer.ToJsonString(localDateTime, CompositeResolver.Create(NodaFormatters.LocalDateTimeFormatter));

            Assert.Equal(jsonDateTime, jsonLocalDateTime);
        }

        [Fact]
        public void LocalDateTimeFormatter_SerializeNonIso_Throws()
        {
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, CalendarSystem.Coptic);

            Assert.Throws<ArgumentException>(() => JsonSerializer.ToJsonString(localDateTime, CompositeResolver.Create(NodaFormatters.LocalDateTimeFormatter)));
        }

        [Fact]
        public void LocalTimeFormatter()
        {
            var value = LocalTime.FromHourMinuteSecondMillisecondTick(1, 2, 3, 4, 5).PlusNanoseconds(67);
            var json = "\"01:02:03.004000567\"";
            AssertConversions(value, json, NodaFormatters.LocalTimeFormatter);
        }

        [Fact]
        public void RoundtripPeriodFormatter()
        {
            var value = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
            var json = "\"P2DT3H90M\"";
            AssertConversions(value, json, NodaFormatters.RoundtripPeriodFormatter);
        }

        [Fact]
        public void NormalizingIsoPeriodFormatter_RequiresNormalization()
        {
            // Can't use AssertConversions here, as it doesn't round-trip
            var period = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
            var json = JsonSerializer.ToJsonString(period, CompositeResolver.Create(NodaFormatters.NormalizingIsoPeriodFormatter));
            var expectedJson = "\"P2DT4H30M\"";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void NormalizingIsoPeriodFormatter_AlreadyNormalized()
        {
            // This time we're okay as it's already a normalized value.
            var value = Period.FromDays(2) + Period.FromHours(4) + Period.FromMinutes(30);
            var json = "\"P2DT4H30M\"";
            AssertConversions(value, json, NodaFormatters.NormalizingIsoPeriodFormatter);
        }

        [Fact]
        public void ZonedDateTimeFormatter()
        {
            // Deliberately give it an ambiguous local time, in both ways.
            var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            var earlierValue = new ZonedDateTime(new LocalDateTime(2012, 10, 28, 1, 30), zone, Offset.FromHours(1));
            var laterValue = new ZonedDateTime(new LocalDateTime(2012, 10, 28, 1, 30), zone, Offset.FromHours(0));
            var earlierJson = "\"2012-10-28T01:30:00+01 Europe/London\"";
            var laterJson = "\"2012-10-28T01:30:00Z Europe/London\"";
            var converter = NodaFormatters.CreateZonedDateTimeFormatter(DateTimeZoneProviders.Tzdb);

            AssertConversions(earlierValue, earlierJson, converter);
            AssertConversions(laterValue, laterJson, converter);
        }

        [Fact]
        public void OffsetDateTimeFormatter()
        {
            var value = new LocalDateTime(2012, 1, 2, 3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.FromHoursAndMinutes(-1, -30));
            var json = "\"2012-01-02T03:04:05.123456789-01:30\"";
            AssertConversions(value, json, NodaFormatters.OffsetDateTimeFormatter);
        }

        [Fact]
        public void OffsetDateTimeFormatter_WholeHours()
        {
            // Redundantly specify the minutes, so that Javascript can parse it and it's RFC3339-compliant.
            // See issue 284 for details.
            var value = new LocalDateTime(2012, 1, 2, 3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.FromHours(5));
            var json = "\"2012-01-02T03:04:05.123456789+05:00\"";
            AssertConversions(value, json, NodaFormatters.OffsetDateTimeFormatter);
        }

        [Fact]
        public void OffsetDateTimeFormatter_ZeroOffset()
        {
            // Redundantly specify the minutes, so that Javascript can parse it and it's RFC3339-compliant.
            // See issue 284 for details.
            var value = new LocalDateTime(2012, 1, 2, 3, 4, 5).PlusNanoseconds(123456789).WithOffset(Offset.Zero);
            var json = "\"2012-01-02T03:04:05.123456789Z\"";
            AssertConversions(value, json, NodaFormatters.OffsetDateTimeFormatter);
        }

        [Fact]
        public void Duration_WholeSeconds()
        {
            AssertConversions(Duration.FromHours(48), "\"48:00:00\"", NodaFormatters.DurationFormatter);
        }

        [Fact]
        public void Duration_FractionalSeconds()
        {
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromNanoseconds(123456789), "\"48:00:03.123456789\"", NodaFormatters.DurationFormatter);
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1230000), "\"48:00:03.123\"", NodaFormatters.DurationFormatter);
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1234000), "\"48:00:03.1234\"", NodaFormatters.DurationFormatter);
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(12345), "\"48:00:03.0012345\"", NodaFormatters.DurationFormatter);
        }

        [Fact]
        public void Duration_MinAndMaxValues()
        {
            AssertConversions(Duration.FromTicks(long.MaxValue), "\"256204778:48:05.4775807\"", NodaFormatters.DurationFormatter);
            AssertConversions(Duration.FromTicks(long.MinValue), "\"-256204778:48:05.4775808\"", NodaFormatters.DurationFormatter);
        }

        /// <summary>
        /// The pre-release converter used either 3 or 7 decimal places for fractions of a second; never less.
        /// This test checks that the "new" converter (using DurationPattern) can still parse the old output.
        /// </summary>
        [Fact]
        public void Duration_ParsePartialFractionalSecondsWithTrailingZeroes()
        {
            var parsed = JsonSerializer.Deserialize<Duration>("\"25:10:00.1234000\"", CompositeResolver.Create(NodaFormatters.DurationFormatter));
            Assert.Equal(Duration.FromHours(25) + Duration.FromMinutes(10) + Duration.FromTicks(1234000), parsed);
        }
    }
}
