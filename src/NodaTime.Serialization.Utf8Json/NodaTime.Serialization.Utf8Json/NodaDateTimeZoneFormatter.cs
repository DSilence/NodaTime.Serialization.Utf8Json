using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaDateTimeZoneFormatter : IJsonFormatter<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider _provider;

        public NodaDateTimeZoneFormatter(IDateTimeZoneProvider provider)
        {
            _provider = provider;
        }

        public void Serialize(ref JsonWriter writer, DateTimeZone value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteString(value.Id);
        }

        public DateTimeZone Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var timeZoneId = reader.ReadString();
            return _provider[timeZoneId];
        }
    }
}