using System;
using NodaTime;
using Utf8Json;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaLocalDateFormatter : IJsonFormatter<LocalDate>
    {
        private readonly Action<LocalDate> _validator;

        public NodaLocalDateFormatter(Action<LocalDate> validator = null)
        {
            _validator = validator;
        }

        public void Serialize(ref JsonWriter writer, LocalDate value, IJsonFormatterResolver formatterResolver)
        {
            _validator?.Invoke(value);
            writer.WriteRawUnsafe((byte)'\"');
            var year = value.Year;
            var month = value.Month;
            var day = value.Day;
            if (year < 10)
            {
                writer.WriteRawUnsafe((byte)'0');
                writer.WriteRawUnsafe((byte)'0');
                writer.WriteRawUnsafe((byte)'0');
            }
            else if (year < 100)
            {
                writer.WriteRawUnsafe((byte)'0');
                writer.WriteRawUnsafe((byte)'0');
            }
            else if (year < 1000)
            {
                writer.WriteRawUnsafe((byte)'0');
            }
            writer.WriteInt32(year);
            writer.WriteRawUnsafe((byte)'-');

            if (month < 10)
            {
                writer.WriteRawUnsafe((byte)'0');
            }
            writer.WriteInt32(month);
            writer.WriteRawUnsafe((byte)'-');

            if (day < 10)
            {
                writer.WriteRawUnsafe((byte)'0');
            }
            writer.WriteInt32(day);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public LocalDate Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var array = str.Array;
            var i = str.Offset;
            var len = str.Count;

            switch (len)
            {
                // YYYY
                case 4:
                {
                    var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 + (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                    return new LocalDate(y, 1, 1);
                }
                // YYYY-MM
                case 7:
                {
                    var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 + (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                    if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                    var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                    return new LocalDate(y, m, 1);
                }
                // YYYY-MM-DD
                case 10:
                {
                    var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 + (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                    if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                    var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                    if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                    var d = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                    return new LocalDate(y, m, d);
                }
                default:
                    Exceptions.ThrowInvalidDateTimeFormat(str);
                    break;
            }

            return default(LocalDate);
        }
    }
}
