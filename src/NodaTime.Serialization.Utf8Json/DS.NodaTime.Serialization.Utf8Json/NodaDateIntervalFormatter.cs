using NodaTime.Serialization.Utf8Json.Dto;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaDateIntervalFormatter : IJsonFormatter<DateInterval>
    {
        public void Serialize(ref JsonWriter writer, DateInterval value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            formatterResolver.GetFormatterWithVerify<DateIntervalDto>().Serialize(ref writer, new DateIntervalDto
            {
                Start = value.Start,
                End = value.End
            }, formatterResolver);
        }

        public DateInterval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            var dto = formatterResolver.GetFormatterWithVerify<DateIntervalDto>().Deserialize(ref reader, formatterResolver);
            return new DateInterval(dto.Start, dto.End);
        }
    }
}
