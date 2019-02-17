using System;
using NodaTime;
using Utf8Json;
using Utf8Json.Internal;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaDurationFormatter : IJsonFormatter<Duration>
    {
        public void Serialize(ref JsonWriter writer, Duration value, IJsonFormatterResolver formatterResolver)
        {
            var hour = Math.Abs(value.Days) * 24 + Math.Abs(value.Hours);
            var minute = Math.Abs(value.Minutes);
            var second = Math.Abs(value.Seconds);
            var nanosec = Math.Abs(value.SubsecondNanoseconds);

            writer.WriteRaw((byte)'\"');
            if (value < Duration.Zero)
            {
                writer.WriteRaw((byte) '-');
            }
            writer.WriteInt32(hour);
            const int baseLength = 6 + 1; // :{Minute}:{Second} + quotation
            writer.EnsureCapacity(baseLength + ((value.SubsecondNanoseconds == 0) ? 0 : NanosecLength));
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
            writer.WriteRawUnsafe((byte)'\"');
        }

        public Duration Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var array = str.Array;
            var i = str.Offset;
            var readCount = 0;
            var minus = array[i] == '-';
            if (minus)
            {
                i++;
            }
            var hour = NumberConverter.ReadInt32(array, i, out readCount);
            i += readCount;
            if (array[i++] != (byte)':') Exceptions.ThrowInvalidDateTimeFormat(str);
            var minute = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');
            if (array[i++] != (byte)':') Exceptions.ThrowInvalidDateTimeFormat(str);
            var second = (array[i++] - (byte)'0') * 10 + (array[i++] - (byte)'0');

            if (i >= array.Length - 1)
            {
                if (minus)
                {
                    return Duration.FromHours(-hour).Plus(Duration.FromMinutes(-minute))
                        .Plus(Duration.FromSeconds(-second));
                }

                return Duration.FromHours(hour).Plus(Duration.FromMinutes(minute))
                    .Plus(Duration.FromSeconds(second));
            }

            long nanoseconds = 0;
            if (array[i] == (byte)'.')
            {
                nanoseconds = NumberConverter.ReadInt64(array, ++i, out readCount);
                i += readCount;
                // account for trailing zeroes
                var times = 9 - readCount;
                for (var j = 0; j < times; j++)
                {
                    nanoseconds = nanoseconds * 10;
                }
            }

            if (minus)
            {
                return nanoseconds > 0
                    ? Duration.FromHours(-hour).Plus(Duration.FromMinutes(-minute))
                        .Plus(Duration.FromSeconds(-second).Plus(Duration.FromNanoseconds(-nanoseconds)))
                    : Duration.FromHours(-hour).Plus(Duration.FromMinutes(-minute)).Plus(Duration.FromSeconds(-second));
            }
            return nanoseconds > 0
                ? Duration.FromHours(hour).Plus(Duration.FromMinutes(minute))
                    .Plus(Duration.FromSeconds(second).Plus(Duration.FromNanoseconds(nanoseconds)))
                : Duration.FromHours(hour).Plus(Duration.FromMinutes(minute)).Plus(Duration.FromSeconds(second));
        }
    }
}
