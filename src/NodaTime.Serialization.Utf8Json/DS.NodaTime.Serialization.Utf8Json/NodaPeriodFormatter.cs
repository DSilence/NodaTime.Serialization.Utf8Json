using NodaTime;
using Utf8Json;
using Utf8Json.Internal;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaPeriodFormatter: IJsonFormatter<Period>
    {
        private readonly bool _isNormalizing;

        public NodaPeriodFormatter(bool isNormalizing)
        {
            _isNormalizing = isNormalizing;
        }

        public void Serialize(ref JsonWriter writer, Period value, IJsonFormatterResolver formatterResolver)
        {
            var writeValue = _isNormalizing ? value.Normalize() : value;
            writer.WriteRaw((byte) '"');
            writer.WriteRaw((byte)'P');
            if (writeValue.HasDateComponent)
            {
                if (writeValue.Years > 0)
                {
                    writer.WriteInt32(writeValue.Years);
                    writer.WriteRaw((byte)'Y');
                }

                if (writeValue.Months > 0)
                {
                    writer.WriteInt32(writeValue.Months);
                    writer.WriteRaw((byte)'M');
                }

                if (writeValue.Weeks > 0)
                {
                    writer.WriteInt32(writeValue.Weeks);
                    writer.WriteRaw((byte)'W');
                }

                if (writeValue.Days > 0)
                {
                    writer.WriteInt32(writeValue.Days);
                    writer.WriteRaw((byte)'D');
                }
            }

            if (writeValue.HasTimeComponent)
            {
                writer.WriteRaw((byte) 'T');
                if (writeValue.Hours > 0)
                {
                    writer.WriteInt64(writeValue.Hours);
                    writer.WriteRaw((byte)'H');
                }

                if (writeValue.Minutes > 0)
                {
                    writer.WriteInt64(writeValue.Minutes);
                    writer.WriteRaw((byte)'M');
                }

                if (writeValue.Seconds > 0)
                {
                    writer.WriteInt64(writeValue.Seconds);
                    writer.WriteRaw((byte)'S');
                }
            }
            writer.WriteRaw((byte) '"');
        }

        public Period Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentRaw();
            var i = str.Offset;
            var periodBuilder = new PeriodBuilder();
            if (str.Array[i++] != 'P')
            {
                Exceptions.ThrowInvalidPeriodFormat(str);
            }

            while (str.Array[i] != 'T' && i - str.Offset < str.Count)
            {
                var amount = NumberConverter.ReadInt32(str.Array, i, out var readCount);
                i += readCount;
                var unit = str.Array[i++];
                switch (unit)
                {
                    case (byte)'Y':
                        periodBuilder.Years = amount;
                        break;
                    case (byte)'M':
                        periodBuilder.Months = amount;
                        break;
                    case (byte)'W':
                        periodBuilder.Weeks = amount;
                        break;
                    case (byte)'D':
                        periodBuilder.Days = amount;
                        break;
                    default:
                        Exceptions.ThrowInvalidPeriodFormat(str);
                        break;
                }
            }

            if (str.Array[i++] == 'T')
            {
                while (i - str.Offset < str.Count)
                {
                    var amount = NumberConverter.ReadInt64(str.Array, i, out var readCount);
                    i += readCount;
                    var unit = str.Array[i++];
                    switch (unit)
                    {
                        case (byte)'H':
                            periodBuilder.Hours = amount;
                            break;
                        case (byte)'M':
                            periodBuilder.Minutes = amount;
                            break;
                        case (byte)'S':
                            periodBuilder.Seconds = amount;
                            break;
                        default:
                            Exceptions.ThrowInvalidPeriodFormat(str);
                            break;
                    }
                }
            }
            return periodBuilder.Build();
        }
    }
}
