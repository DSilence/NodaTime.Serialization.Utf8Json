using System;
using NodaTime.Text;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaFormatters
    {
        /// <summary>
        /// Formatter for instants, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks, and
        /// specifying 'Z' at the end to show it's effectively in UTC.
        /// </summary>
        public static IJsonFormatter InstantFormatter { get; }
            = new NodaPatternFormatter<Instant>(InstantPattern.ExtendedIso);

        /// <summary>
        /// Formatter for local dates, using the ISO-8601 date pattern.
        /// </summary>
        public static IJsonFormatter LocalDateFormatter { get; }
            = new NodaPatternFormatter<LocalDate>(
                LocalDatePattern.Iso, CreateIsoValidator<LocalDate>(x => x.Calendar));

        /// <summary>
        /// Formatter for local dates and times, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks.
        /// No time zone designator is applied.
        /// </summary>
        public static IJsonFormatter LocalDateTimeFormatter { get; }
            = new NodaPatternFormatter<LocalDateTime>(
                LocalDateTimePattern.ExtendedIso, CreateIsoValidator<LocalDateTime>(x => x.Calendar));

        /// <summary>
        /// Formatter for local times, using the ISO-8601 time pattern, extended as required to accommodate milliseconds and ticks.
        /// </summary>
        public static IJsonFormatter LocalTimeFormatter { get; }
            = new NodaPatternFormatter<LocalTime>(LocalTimePattern.ExtendedIso);

        /// <summary>
        /// Formatter for intervals. This must be used in a serializer which also has an instant converter.
        /// </summary>
        public static IJsonFormatter IntervalFormatter { get; } = new NodaIntervalFormatter();

        /// <summary>
        /// Formatter for intervals using extended ISO-8601 format, as output by <see cref="Interval.ToString"/>.
        /// </summary>
        public static IJsonFormatter IsoIntervalFormatter { get; }
            = new NodaIsoIntervalFormatter();

        /// <summary>
        /// Formatter for date intervals. This must be used in a serializer which also has a local date converter.
        /// </summary>
        public static IJsonFormatter DateIntervalFormatter { get; } = new NodaDateIntervalFormatter();

        /// <summary>
        /// Formatter for date intervals using ISO-8601 format, as defined by <see cref="LocalDatePattern.Iso"/>.
        /// </summary>
        public static IJsonFormatter IsoDateIntervalFormatter { get; }
            = new NodaIsoDateIntervalFormatter();

        /// <summary>
        /// Formatter for offsets.
        /// </summary>
        public static IJsonFormatter OffsetFormatter { get; }
            = new NodaPatternFormatter<Offset>(OffsetPattern.GeneralInvariant);

        /// <summary>
        /// Formatter for offset date/times.
        /// </summary>
        public static IJsonFormatter OffsetDateTimeFormatter { get; } =
            new NodaPatternFormatter<OffsetDateTime>(
                OffsetDateTimePattern.Rfc3339, CreateIsoValidator<OffsetDateTime>(x => x.Calendar));

        /// <summary>
        /// Creates a converter for zoned date/times, using the given time zone provider.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing.</param>
        /// <returns>A converter to handle <see cref="ZonedDateTime"/>.</returns>
        public static IJsonFormatter<ZonedDateTime> CreateZonedDateTimeFormatter(IDateTimeZoneProvider provider) =>
            new NodaPatternFormatter<ZonedDateTime>(
                ZonedDateTimePattern.CreateWithInvariantCulture("uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z", provider),
                CreateIsoValidator<ZonedDateTime>(x => x.Calendar));


        /// <summary>
        /// Converter for durations.
        /// </summary>
        public static IJsonFormatter DurationFormatter { get; }
            = new NodaPatternFormatter<Duration>(DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFFFF"));

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
            Preconditions.CheckArgument(calendar == CalendarSystem.Iso,
                "Values of type {0} must (currently) use the ISO calendar in order to be serialized.",
                typeof(T).Name);
        };
    }
}