using System;
using System.Text;
using NodaTime;
using Utf8Json;
using Utf8Json.Internal;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaOffsetDateTimeFormatter : IJsonFormatter<OffsetDateTime>
    {
        private readonly Action<OffsetDateTime> _validator;

        public NodaOffsetDateTimeFormatter(Action<OffsetDateTime> validator = null)
        {
            _validator = validator;
        }

        public void Serialize(ref JsonWriter writer, OffsetDateTime value, IJsonFormatterResolver formatterResolver)
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

            var localOffset = value.Offset;
            if (localOffset == Offset.Zero)
            {
                writer.WriteRaw((byte)'Z');
            }
            else
            {
                var minus = (localOffset < Offset.Zero);
                var totalSeconds = localOffset.Seconds;
                if (minus) totalSeconds = -totalSeconds;

                var h = Math.DivRem(totalSeconds, 3600, out var remainder);
                var m = remainder / 60;
                writer.WriteRawUnsafe(minus ? (byte)'-' : (byte)'+');
                if (h < 10)
                {
                    writer.WriteRawUnsafe((byte)'0');
                }

                writer.WriteInt32(h);
                writer.WriteRawUnsafe((byte)':');
                if (m < 10)
                {
                    writer.WriteRawUnsafe((byte)'0');
                }

                writer.WriteInt32(m);
            }
            writer.WriteRawUnsafe((byte)'\"');
        }

        public OffsetDateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
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
                return new OffsetDateTime(new LocalDateTime(y, 1, 1, 0, 0, 0), Offset.Zero);
            }

            // YYYY-MM
            if (len == 7)
            {
                var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                        (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                return new OffsetDateTime(new LocalDateTime(y, m, 1, 0, 0, 0), Offset.Zero);
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
                return new OffsetDateTime(new LocalDateTime(y, m, d, 0, 0, 0), Offset.Zero);
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

            long nanoseconds = 0;
            if (i < array.Length - 1)
            {
                nanoseconds = NumberConverter.ReadInt64(array, i + 1, out var readCount);
                i = i + readCount + 1;
            }

            OffsetDateTime offsetDateTime;

            if (i < to && array[i] == '-' || array[i] == '+')
            {
                if (!(i + 5 < to)) Exceptions.ThrowInvalidDateTimeFormat(str);

                var minus = array[i++] == '-';

                var h = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                i++;
                var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');

                var offset = minus ? Offset.FromHoursAndMinutes(-h, -m) : Offset.FromHoursAndMinutes(h, m);

                offsetDateTime = new OffsetDateTime(new LocalDateTime(year, month, day, hour, minute, second), offset);
            }

            else
            {
                offsetDateTime = new OffsetDateTime(new LocalDateTime(year, month, day, hour, minute, second),
                    Offset.Zero);
            }

            if (nanoseconds > 0)
            {
                offsetDateTime = offsetDateTime.PlusNanoseconds(nanoseconds);
            }

            return offsetDateTime;
        }
    }
}
