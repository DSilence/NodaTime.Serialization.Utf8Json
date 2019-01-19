using System;
using NodaTime;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

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
            const int baseLength = 19 + 2; // {YEAR}-{MONTH}-{DAY}T{Hour}:{Minute}:{Second} + quotation

            // +{Hour}:{Minute}
            writer.EnsureCapacity(baseLength + ((value.NanosecondOfSecond == 0) ? 0 : NanosecLength));

            writer.WriteRawUnsafe((byte)'\"');
            WriteDate(ref writer, value.Date, formatterResolver);
            writer.WriteRawUnsafe((byte)'T');
            WriteTime(ref writer, value.TimeOfDay, formatterResolver);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public LocalDateTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            var date = ReadDate(str, formatterResolver, ref i);
            if (str.Array[i++] != (byte)'T') Exceptions.ThrowInvalidDateTimeFormat(str);
            var time = ReadTime(str, formatterResolver, ref i);
            return date.At(time);
        }
    }
}
