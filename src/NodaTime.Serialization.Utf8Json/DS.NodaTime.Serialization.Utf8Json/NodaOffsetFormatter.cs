using NodaTime;
using Utf8Json;
using static DS.NodaTime.Serialization.Utf8Json.Helpers.NodaIsoDateTimeHandler;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaOffsetFormatter : IJsonFormatter<Offset>
    {
        public void Serialize(ref JsonWriter writer, Offset value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRawUnsafe((byte)'\"');
            WriteOffset(ref writer, value, formatterResolver);
            writer.WriteRawUnsafe((byte)'\"');
        }

        public Offset Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var str = reader.ReadStringSegmentUnsafe();
            var i = str.Offset;
            return ReadOffset(str, formatterResolver, ref i);
        }
    }
}
