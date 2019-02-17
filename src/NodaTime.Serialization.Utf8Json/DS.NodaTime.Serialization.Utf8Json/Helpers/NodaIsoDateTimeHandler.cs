using System;
using NodaTime;
using Utf8Json;
using Utf8Json.Internal;

namespace DS.NodaTime.Serialization.Utf8Json.Helpers
{
    internal static class NodaIsoDateTimeHandler
    {
        public const int NanosecLength = 10;

        internal static void WriteDate(ref JsonWriter writer,in LocalDate value, IJsonFormatterResolver formatterResolver)
        {
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
        }

        internal static void WriteTime(ref JsonWriter writer,in LocalTime value, IJsonFormatterResolver formatterResolver)
        {
            var hour = value.Hour;
            var minute = value.Minute;
            var second = value.Second;
            var nanosec = value.NanosecondOfSecond;
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
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 1_000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 10_000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 100_000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }
                else if (nanosec < 1_000_000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }

                else if (nanosec < 10_000_000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                    writer.WriteRawUnsafe((byte)'0');
                }

                else if (nanosec < 100_000_000)
                {
                    writer.WriteRawUnsafe((byte)'0');
                }

                while (nanosec % 10 == 0)
                {
                    nanosec = nanosec / 10;
                }

                writer.WriteInt32(nanosec);
            }
        }

        internal static void WriteOffset(ref JsonWriter writer, Offset value,
            IJsonFormatterResolver formatterResolver, bool isExtendedIso = false)
        {
            if (value == Offset.Zero)
            {
                writer.WriteRawUnsafe((byte)'Z');
            }
            else
            {
                var minus = (value < Offset.Zero);
                var totalSeconds = value.Seconds;
                if (minus) totalSeconds = -totalSeconds;

                var h = Math.DivRem(totalSeconds, 3600, out var remainder);
                var m = remainder / 60;
                writer.WriteRawUnsafe(minus ? (byte)'-' : (byte)'+');
                if (h < 10)
                {
                    writer.WriteRawUnsafe((byte)'0');
                }

                writer.WriteInt32(h);
                if (!isExtendedIso || m > 0)
                {
                    writer.WriteRawUnsafe((byte)':');
                    if (m < 10)
                    {
                        writer.WriteRawUnsafe((byte)'0');
                    }

                    writer.WriteInt32(m);
                }
            }
        }

        internal static LocalDate ReadDate(in ArraySegment<byte> str, IJsonFormatterResolver formatterResolver, ref int i)
        {
            var array = str.Array;
            var len = str.Count;

            // YYYY
            if (len == 4)
            {
                var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                        (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                return new LocalDate(y, 1, 1);
            }

            // YYYY-MM
            if (len == 7)
            {
                var y = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                        (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
                var m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
                return new LocalDate(y, m, 1);
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
                return new LocalDate(y, m, d);
            }

            var year = (array[i++] - (byte)'0') * 1000 + (array[i++] - (byte)'0') * 100 +
                       (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
            var month = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)'-') Exceptions.ThrowInvalidDateTimeFormat(str);
            var day = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            return new LocalDate(year, month, day);
        }

        internal static LocalTime ReadTime(in ArraySegment<byte> str, IJsonFormatterResolver formatterResolver, ref int i)
        {
            var array = str.Array;
            var hour = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)':') Exceptions.ThrowInvalidDateTimeFormat(str);
            var minute = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)':') Exceptions.ThrowInvalidDateTimeFormat(str);
            var second = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');

            if (i >= array.Length - 1)
            {
                return new LocalTime(hour, minute, second);
            }

            long nanoseconds = 0;
            if (array[i] == (byte)'.')
            {
                nanoseconds = NumberConverter.ReadInt64(array, ++i, out var readCount);
                i += readCount;
                // account for trailing zeroes
                var times = 9 - readCount;
                for (var j = 0; j < times; j++)
                {
                    nanoseconds = nanoseconds * 10;
                }
            }

            return nanoseconds > 0
                ? LocalTime.FromHourMinuteSecondNanosecond(hour, minute, second, nanoseconds)
                : new LocalTime(hour, minute, second);
        }

        internal static Offset ReadOffset(in ArraySegment<byte> str, IJsonFormatterResolver formatterResolver, ref int i)
        {
            var array = str.Array;
            var to = str.Offset + str.Count;
            if (array[i] == 'Z')
            {
                // skip z and space
                i++;
                i++;
                return Offset.Zero;
            }
            if ((i >= to || array[i] != '-') && array[i] != '+')
            {
                Exceptions.ThrowInvalidDateTimeFormat(str);
            }

            if (!(i + 5 < to)) Exceptions.ThrowInvalidDateTimeFormat(str);

            var minus = array[i++] == '-';

            var h = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            var m = 0;
            if (array[i++] == ':')
            {
                m = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            }

            var offset = minus ? Offset.FromHoursAndMinutes(-h, -m) : Offset.FromHoursAndMinutes(h, m);
            return offset;
        }
    }
}
