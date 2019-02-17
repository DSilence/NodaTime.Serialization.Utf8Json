using System;
using NodaTime;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

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
            const int length = 10 + 2; // {YEAR}-{MONTH}-{DAY} + quotation

            writer.EnsureCapacity(length);

            writer.WriteRawUnsafe((byte)'\"');
            WriteDate(ref writer, value, formatterResolver);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public LocalDate Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            var date = ReadDate(str, formatterResolver, ref i);

            return date;
        }
    }
}
