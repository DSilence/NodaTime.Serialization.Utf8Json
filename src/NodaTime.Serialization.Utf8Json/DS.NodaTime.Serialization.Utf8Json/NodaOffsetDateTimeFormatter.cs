using System;
using NodaTime;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

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

            const int baseLength = 19 + 2; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation
            const int offsetLength = 6;

            // +{Hour}:{Minute}
            writer.EnsureCapacity(baseLength + ((value.NanosecondOfSecond == 0) ? 0 : NanosecLength) + offsetLength);
            writer.WriteRawUnsafe((byte)'\"');
            WriteDate(ref writer, value.Date, formatterResolver);
            writer.WriteRawUnsafe((byte)'T');
            WriteTime(ref writer, value.TimeOfDay, formatterResolver);
            WriteOffset(ref writer, value.Offset, formatterResolver);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public OffsetDateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            var date = ReadDate(str, formatterResolver, ref i);
            if (str.Array[i++] != (byte)'T') Exceptions.ThrowInvalidDateTimeFormat(str);
            var time = ReadTime(str, formatterResolver, ref i);
            var offset = ReadOffset(str, formatterResolver, ref i);
            return date.At(time).WithOffset(offset);
        }
    }
}
