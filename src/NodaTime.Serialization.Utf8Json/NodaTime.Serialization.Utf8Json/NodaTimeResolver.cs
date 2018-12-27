using System;
using System.Collections.Generic;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaTimeResolver : IJsonFormatterResolver
    {
        private readonly Dictionary<Type, IJsonFormatter> _resolverCache;

        public NodaTimeResolver(
            IDateTimeZoneProvider dateTimeZoneProvider,
            bool isoIntervals = false,
            bool isoDateIntervals = false,
            bool normalizedIsoPeriods = false)
        {
            NodaFormatters.CreateDateTimeZoneFormatter(dateTimeZoneProvider);

            _resolverCache = new Dictionary<Type, IJsonFormatter>
            {
                {typeof(ZonedDateTime), NodaFormatters.CreateZonedDateTimeFormatter(dateTimeZoneProvider)},
                {typeof(DateTimeZone), NodaFormatters.CreateDateTimeZoneFormatter(dateTimeZoneProvider)},
                {typeof(Interval), isoIntervals ? NodaFormatters.IsoIntervalFormatter : NodaFormatters.IsoDateIntervalFormatter},
                {typeof(DateInterval), isoDateIntervals ? NodaFormatters.IsoDateIntervalFormatter : NodaFormatters.DateIntervalFormatter},
                {typeof(Period), normalizedIsoPeriods ? NodaFormatters.NormalizingIsoPeriodFormatter : NodaFormatters.RoundtripPeriodFormatter}
            };
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter ?? GetResolverFormatter<T>();
        }

        private IJsonFormatter<T> GetResolverFormatter<T>()
        {
            return _resolverCache.TryGetValue(typeof(T), out var formatter) ? (IJsonFormatter<T>) formatter : null;
        }

        private static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> Formatter;

            static FormatterCache()
            {
                Formatter = (IJsonFormatter<T>)NodaTimeResolverHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class NodaTimeResolverHelper
    {
        static readonly Dictionary<Type, IJsonFormatter> DefaultFormatterMap = new Dictionary<Type, IJsonFormatter>
        {
            {typeof(Instant), NodaFormatters.InstantFormatter},
            {typeof(LocalDate), NodaFormatters.LocalDateFormatter},
            {typeof(LocalTime), NodaFormatters.LocalTimeFormatter},
            {typeof(LocalDateTime), NodaFormatters.LocalDateTimeFormatter},
            {typeof(Offset), NodaFormatters.OffsetFormatter},
            {typeof(OffsetDateTime), NodaFormatters.OffsetDateTimeFormatter},
            {typeof(Duration), NodaFormatters.DurationFormatter},
        };

        internal static object GetFormatter(Type t)
        {
            IJsonFormatter formatter;
            return DefaultFormatterMap.TryGetValue(t, out formatter) ? formatter : null;
        }
    }
}