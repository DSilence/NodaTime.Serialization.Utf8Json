using NodaTime;
using NodaTime.Utility;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaInstantFormatter : IJsonFormatter<Instant>
    {
        public void Serialize(ref JsonWriter writer, Instant value, IJsonFormatterResolver formatterResolver)
        {
            const int baseLength = 19 + 2; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation
            const int offsetLength = 1;
            var offsetDateTime = value.WithOffset(Offset.Zero);

            // +{Hour}:{Minute}
            writer.EnsureCapacity(baseLength + ((offsetDateTime.NanosecondOfSecond == 0) ? 0 : NanosecLength) + offsetLength);
            writer.WriteRawUnsafe((byte)'\"');
            WriteDate(ref writer, offsetDateTime.Date, formatterResolver);
            writer.WriteRawUnsafe((byte)'T');
            WriteTime(ref writer, offsetDateTime.TimeOfDay, formatterResolver, true);
            WriteOffset(ref writer, offsetDateTime.Offset, formatterResolver);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public Instant Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            var date = ReadDate(str, formatterResolver, ref i);
            if (str.Array[i++] != (byte)'T') Exceptions.ThrowInvalidDateTimeFormat(str);
            var time = ReadTime(str, formatterResolver, ref i, true);
            var offset = ReadOffset(str, formatterResolver, ref i);
            if (offset != Offset.Zero)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date without offset.");
            }
            return date.At(time).WithOffset(offset).ToInstant();
        }
    }
}
