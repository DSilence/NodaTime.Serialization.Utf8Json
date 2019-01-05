using System;
using DS.NodaTime.Serialization.Utf8Json.Enums;
using NodaTime;
using NodaTime.Text;
using Utf8Json;
using Utf8Json.Formatters;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaFormatters
    {
        /// <summary>
        /// Formatter for instants, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks, and
        /// specifying 'Z' at the end to show it's effectively in UTC.
        /// </summary>
        public static IJsonFormatter<Instant> InstantFormatter { get; }
            = new NodaPatternFormatter<Instant>(InstantPattern.ExtendedIso);

        /// <summary>
        /// Formatter for nullable instants, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks, and
        /// specifying 'Z' at the end to show it's effectively in UTC.
        /// </summary>
        public static IJsonFormatter<Instant?> NullableInstantFormatter { get; }
            = new StaticNullableFormatter<Instant>(InstantFormatter);

        /// <summary>
        /// Formatter for local dates, using the ISO-8601 date pattern.
        /// </summary>
        public static IJsonFormatter<LocalDate> LocalDateFormatter { get; }
            = new NodaPatternFormatter<LocalDate>(
                LocalDatePattern.Iso, CreateIsoValidator<LocalDate>(x => x.Calendar));

        /// <summary>
        /// Formatter for nullable local dates, using the ISO-8601 date pattern.
        /// </summary>
        public static IJsonFormatter<LocalDate?> NullableLocalDateFormatter { get; }
            = new StaticNullableFormatter<LocalDate>(LocalDateFormatter);

        /// <summary>
        /// Formatter for local dates and times, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks.
        /// No time zone designator is applied.
        /// </summary>
        public static IJsonFormatter<LocalDateTime> LocalDateTimeFormatter { get; }
            = new NodaPatternFormatter<LocalDateTime>(
                LocalDateTimePattern.ExtendedIso, CreateIsoValidator<LocalDateTime>(x => x.Calendar));

        /// <summary>
        /// Formatter for nullable local dates and times, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks.
        /// No time zone designator is applied.
        /// </summary>
        public static IJsonFormatter<LocalDateTime?> NullableLocalDateTimeFormatter { get; }
            = new StaticNullableFormatter<LocalDateTime>(LocalDateTimeFormatter);

        /// <summary>
        /// Formatter for local times, using the ISO-8601 time pattern, extended as required to accommodate milliseconds and ticks.
        /// </summary>
        public static IJsonFormatter<LocalTime> LocalTimeFormatter { get; }
            = new NodaPatternFormatter<LocalTime>(LocalTimePattern.ExtendedIso);

        /// <summary>
        /// Formatter for nullable local times, using the ISO-8601 time pattern, extended as required to accommodate milliseconds and ticks.
        /// </summary>
        public static IJsonFormatter<LocalTime?> NullableLocalTimeFormatter { get; }
            = new StaticNullableFormatter<LocalTime>(LocalTimeFormatter);

        /// <summary>
        /// Formatter for intervals. This must be used in a serializer which also has an instant converter.
        /// </summary>
        public static IJsonFormatter<Interval> CreateIntervalFormatter(NameHandling nameHandling) =>
            new NodaIntervalFormatter(GetNameMutator(nameHandling));

        /// <summary>
        /// Formatter for intervals using extended ISO-8601 format, as output by <see cref="Interval.ToString"/>.
        /// </summary>
        public static IJsonFormatter<Interval> IsoIntervalFormatter { get; }
            = new NodaIsoIntervalFormatter();

        /// <summary>
        /// Formatter for nullable intervals.
        /// </summary>
        public static IJsonFormatter<Interval?> NullableIntervalFormatter { get; } = new NullableFormatter<Interval>();

        /// <summary>
        /// Formatter for date intervals. This must be used in a serializer which also has a local date converter.
        /// </summary>
        public static IJsonFormatter<DateInterval> CreateDateIntervalFormatter(NameHandling nameHandling)
            => new NodaDateIntervalFormatter(GetNameMutator(nameHandling));

        /// <summary>
        /// Formatter for date intervals using ISO-8601 format, as defined by <see cref="LocalDatePattern.Iso"/>.
        /// </summary>
        public static IJsonFormatter<DateInterval> IsoDateIntervalFormatter { get; }
            = new NodaIsoDateIntervalFormatter();

        /// <summary>
        /// Formatter for offsets.
        /// </summary>
        public static IJsonFormatter<Offset> OffsetFormatter { get; }
            = new NodaPatternFormatter<Offset>(OffsetPattern.GeneralInvariant);

        /// <summary>
        /// Formatter for nullable offsets.
        /// </summary>
        public static IJsonFormatter<Offset?> NullableOffsetFormatter { get; }
            = new StaticNullableFormatter<Offset>(OffsetFormatter);

        /// <summary>
        /// Formatter for offset date/times.
        /// </summary>
        public static IJsonFormatter<OffsetDateTime> OffsetDateTimeFormatter { get; } =
            new NodaPatternFormatter<OffsetDateTime>(
                OffsetDateTimePattern.Rfc3339, CreateIsoValidator<OffsetDateTime>(x => x.Calendar));

        /// <summary>
        /// Formatter for nullable offset date/times.
        /// </summary>
        public static IJsonFormatter<OffsetDateTime?> NullableOffsetDateTimeFormatter { get; } =
            new StaticNullableFormatter<OffsetDateTime>(OffsetDateTimeFormatter);

        /// <summary>
        /// Creates a formatter for zoned date/times, using the given time zone provider.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing.</param>
        /// <returns>A converter to handle <see cref="ZonedDateTime"/>.</returns>
        public static IJsonFormatter<ZonedDateTime> CreateZonedDateTimeFormatter(IDateTimeZoneProvider provider) =>
            new NodaPatternFormatter<ZonedDateTime>(
                ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z",
                    provider),
                CreateIsoValidator<ZonedDateTime>(x => x.Calendar));


        /// <summary>
        /// Creates a nullable formatter for zoned date/times, using the given time zone provider.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing.</param>
        /// <returns>A converter to handle <see cref="ZonedDateTime"/>.</returns>
        public static IJsonFormatter<ZonedDateTime?> CreateNullableZonedDateTimeFormatter(
            IDateTimeZoneProvider provider) =>
            new StaticNullableFormatter<ZonedDateTime>(CreateZonedDateTimeFormatter(provider));


        /// <summary>
        /// Formatter for durations.
        /// </summary>
        public static IJsonFormatter<Duration> DurationFormatter { get; }
            = new NodaPatternFormatter<Duration>(DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFFFF"));

        /// <summary>
        /// Formatter for nullable durations.
        /// </summary>
        public static IJsonFormatter<Duration?> NullableDurationFormatter { get; }
            = new StaticNullableFormatter<Duration>(DurationFormatter);

        /// <summary>
        /// Round-tripping converter for periods. Use this when you really want to preserve information,
        /// and don't need interoperability with systems expecting ISO.
        /// </summary>
        public static IJsonFormatter RoundtripPeriodFormatter { get; }
            = new NodaPatternFormatter<Period>(PeriodPattern.Roundtrip);

        /// <summary>
        /// Normalizing ISO converter for periods. Use this when you want compatibility with systems expecting
        /// ISO durations (~= Noda Time periods). However, note that Noda Time can have negative periods. Note that
        /// this converter losees information - after serialization and deserialization, "90 minutes" will become "an hour and 30 minutes".
        /// </summary>
        public static IJsonFormatter NormalizingIsoPeriodFormatter { get; }
            = new NodaPatternFormatter<Period>(PeriodPattern.NormalizingIso);

        /// <summary>
        /// Creates a converter for time zones, using the given provider.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing.</param>
        /// <returns>A converter to handle <see cref="DateTimeZone"/>.</returns>
        public static IJsonFormatter<DateTimeZone> CreateDateTimeZoneFormatter(IDateTimeZoneProvider provider) =>
            new NodaDateTimeZoneFormatter(provider);

        private static Action<T> CreateIsoValidator<T>(Func<T, CalendarSystem> calendarProjection) => value =>
        {
            var calendar = calendarProjection(value);
            // We rely on CalendarSystem.Iso being a singleton here.
            if (calendar != CalendarSystem.Iso)
            {
                throw new ArgumentException(
                    "Values of type {0} must (currently) use the ISO calendar in order to be serialized.",
                    typeof(T).Name);
            }
        };

        private static Func<string, string> GetNameMutator(NameHandling nameHandling)
        {
            switch (nameHandling)
            {
                case NameHandling.Ordinal:
                    return StringMutator.Original;
                case NameHandling.CamelCase:
                    return StringMutator.ToCamelCase;
                case NameHandling.SnakeCase:
                    return StringMutator.ToSnakeCase;
            }
            throw new ArgumentException($"Invalid name handling selected. Expected one of {string.Join(",", Enum.GetNames(typeof(NameHandling)))}");
        }
    }
}
