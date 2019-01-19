using System;
using NodaTime;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaLocalTimeFormatter : IJsonFormatter<LocalTime>
    {
        public void Serialize(ref JsonWriter writer, LocalTime value, IJsonFormatterResolver formatterResolver)
        {
            const int baseLength = 8 + 2; // {Hour}:{Minute}:{Second} + quotation
            writer.EnsureCapacity(baseLength + ((value.NanosecondOfSecond == 0) ? 0 : NanosecLength));
            writer.WriteRawUnsafe((byte)'\"');
            WriteTime(ref writer, value, formatterResolver);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public LocalTime Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            var time = ReadTime(str, formatterResolver, ref i);
            return time;
        }
    }
}
