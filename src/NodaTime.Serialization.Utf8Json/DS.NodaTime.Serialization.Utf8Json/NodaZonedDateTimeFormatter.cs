using System;
using System.Text;
using NodaTime;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaZonedDateTimeFormatter : IJsonFormatter<ZonedDateTime>
    {
        private readonly IDateTimeZoneProvider _dateTimeZoneProvider;
        private readonly Action<ZonedDateTime> _validator;

        public NodaZonedDateTimeFormatter(IDateTimeZoneProvider dateTimeZoneProvider, Action<ZonedDateTime> validator)
        {
            _dateTimeZoneProvider = dateTimeZoneProvider;
            _validator = validator;
        }

        public void Serialize(ref JsonWriter writer, ZonedDateTime value, IJsonFormatterResolver formatterResolver)
        {
            _validator?.Invoke(value);
            const int baseLength = 19 + 1; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation
            const int offsetLength = 6;

            // +{Hour}:{Minute}
            writer.EnsureCapacity(baseLength + ((value.NanosecondOfSecond == 0) ? 0 : NanosecLength) + offsetLength);

            writer.WriteRawUnsafe((byte)'\"');
            WriteDate(ref writer, value.Date, formatterResolver);
            writer.WriteRawUnsafe((byte)'T');
            WriteTime(ref writer, value.TimeOfDay, formatterResolver);
            WriteOffset(ref writer, value.Offset, formatterResolver, true);
            writer.WriteRawUnsafe((byte)' ');
            writer.WriteRaw(StringEncoding.UTF8.GetBytes(value.Zone.Id));
            writer.WriteRaw((byte)'\"');
        }

        public ZonedDateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            var date = ReadDate(str, formatterResolver, ref i);
            if (str.Array[i++] != (byte)'T') Exceptions.ThrowInvalidDateTimeFormat(str);
            var time = ReadTime(str, formatterResolver, ref i);
            var offset = ReadOffset(str, formatterResolver, ref i);
            var timezoneString = StringEncoding.UTF8.GetString(str.Array, i, str.Count - i + 1);
            var zone = _dateTimeZoneProvider.GetZoneOrNull(timezoneString);
            if (zone == null)
            {
                Exceptions.ThrowInvalidTimeZone(str);
            }
            return new ZonedDateTime(date.At(time), zone, offset);
        }
    }
}
