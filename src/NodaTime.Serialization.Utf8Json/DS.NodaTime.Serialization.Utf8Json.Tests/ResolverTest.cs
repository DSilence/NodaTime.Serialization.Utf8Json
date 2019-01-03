using DS.NodaTime.Serialization.Utf8Json;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    public class FormatterTest
    {
        [Fact]
        public void Resolver_DefaultInterval()
        {
            var configuredResolver = CompositeResolver.Create(new NodaTimeResolver(DateTimeZoneProviders.Tzdb),
                StandardResolver.Default);
            var explicitResolver =
                CompositeResolver.Create(new IJsonFormatter[]
                        {NodaFormatters.IntervalFormatter, NodaFormatters.NullableInstantFormatter},
                    new[] {StandardResolver.Default}
                );

            var interval = new Interval(Instant.FromUnixTimeTicks(1000L), Instant.FromUnixTimeTicks(20000L));
            Assert.Equal(Serialize(interval, explicitResolver),
                Serialize(interval, configuredResolver));
        }

        [Fact]
        public void Resolver_WithIsoIntervalFormatter()
        {
            var configuredResolver = CompositeResolver.Create(new NodaTimeResolver(DateTimeZoneProviders.Tzdb, true), StandardResolver.Default);
            var explicitResolver = CompositeResolver.Create(new IJsonFormatter[] {NodaFormatters.IsoIntervalFormatter},
                new[] {StandardResolver.Default});
            var interval = new Interval(Instant.FromUnixTimeTicks(1000L), Instant.FromUnixTimeTicks(20000L));
            Assert.Equal(Serialize(interval, explicitResolver),
                Serialize(interval, configuredResolver));
        }

        [Fact]
        public void Resolver_DefaultDateInterval()
        {
            var configuredResolver = CompositeResolver.Create(new NodaTimeResolver(DateTimeZoneProviders.Tzdb),
                StandardResolver.Default);
            var explicitResolver =
                CompositeResolver.Create(
                    new IJsonFormatter[] {NodaFormatters.DateIntervalFormatter, NodaFormatters.LocalDateFormatter},
                    new[] {StandardResolver.Default});
            var interval = new DateInterval(new LocalDate(2001, 2, 3), new LocalDate(2004, 5, 6));
            Assert.Equal(Serialize(interval, explicitResolver),
                Serialize(interval, configuredResolver));
        }

        [Fact]
        public void Resolver_WithIsoDateIntervalFormatter()
        {
            var configuredResolver = CompositeResolver.Create(
                new NodaTimeResolver(DateTimeZoneProviders.Tzdb, isoDateIntervals: true),
                StandardResolver.Default);
            var explicitResolver =
                CompositeResolver.Create(NodaFormatters.IsoDateIntervalFormatter);
            var interval = new DateInterval(new LocalDate(2001, 2, 3), new LocalDate(2004, 5, 6));
            Assert.Equal(Serialize(interval, explicitResolver),
                Serialize(interval, configuredResolver));
        }

        private static string Serialize<T>(T value, IJsonFormatterResolver jsonFormatterResolver)
        {
            return JsonSerializer.ToJsonString(value, jsonFormatterResolver);
        }
    }
}
