using System;
using NodaTime;
using Utf8Json;
using Utf8Json.Internal;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaLocalDateTimeFormatter : IJsonFormatter<LocalDateTime>
    {
        private readonly Action<LocalDateTime> _validator;

        public NodaLocalDateTimeFormatter(Action<LocalDateTime> validator = null)
        {
            _validator = validator;
        }

        public void Serialize(ref JsonWriter writer, LocalDateTime value, IJsonFormatterResolver formatterResolver)
        {
            _validator?.Invoke(value);
            var year = value.Year;
            var month = value.Month;
            var day = value.Day;
            var hour = value.Hour;
            var minute = value.Minute;
            var second = value.Second;
            var nanosec = value.NanosecondOfSecond;

            const int baseLength = 19 + 2; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation
            const int nanosecLength = 8; // .{nanoseconds}

            // +{Hour}:{Minute}
            writer.EnsureCapacity(baseLength + ((nanosec == 0) ? 0 : nanosecLength) + 6);

            writer.WriteRawUnsafe((byte)'\"');

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

            writer.WriteRawUnsafe((byte)'T');

            if (hour < 10)
            {
                writer.WriteRawUnsafe((byte)'0');
            }

            writer.WriteInt32(hour);
            writer.WriteRawUnsafe((byte)':');

            if (minute < 10)
            {
                writer.WriteRawUnsafe((byte)'0');
            }

            writer.WriteInt32(minute);
            writer.WriteRawUnsafe((byte)':');

            if (second < 10)
            {
                writer.WriteRawUnsafe((byte)'0');
            }

            writer.WriteInt32(second);

            if (nanosec != 0)
            {
                writer.WriteRawUnsafe((byte)'.');

                if (nanosec < 10)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 100)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 1000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 10000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 100000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 1000000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                }

                writer.WriteInt64(nanosec);
            }

            writer.WriteRawUnsafe((byte)'\"');
        }

        public LocalDateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var array = str.Array;
            var i = str.Offset;
            var len = str.Count;
            var to = str.Offset + str.Count;

            // YYYY
            if (len == 4)
            {
                var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                        (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                return new LocalDateTime(y, 1, 1, 0, 0, 0);
            }

            // YYYY-MM
            if (len == 7)
            {
                var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                        (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                return new LocalDateTime(y, m, 1, 0, 0, 0);
            }

            // YYYY-MM-DD
            if (len == 10)
            {
                var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                        (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                var d = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                return new LocalDateTime(y, m, d, 0, 0, 0);
            }

            // range-first section requires 19
            if (array.Length < 19) Exceptions.ThrowInvalidDateTimeFormat(str);

            var year = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                       (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
            var month = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
            var day = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');

            if (array[i++] != (byte)'T') Exceptions.ThrowInvalidDateTimeFormat(str);

            var hour = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)':') Exceptions.ThrowInvalidDateTimeFormat(str);
            var minute = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)':') Exceptions.ThrowInvalidDateTimeFormat(str);
            var second = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');

            var localDateTime = new LocalDateTime(year, month, day, hour, minute, second);
            if (i < array.Length - 1)
            {
                var nanoseconds = NumberConverter.ReadInt64(array, i + 1, out i);
                if (nanoseconds > 0)
                {
                    localDateTime = localDateTime.PlusNanoseconds(nanoseconds);
                }
            }

            return localDateTime;
        }
    }
}
