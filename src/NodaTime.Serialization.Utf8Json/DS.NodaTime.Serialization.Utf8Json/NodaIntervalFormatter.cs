using NodaTime.Serialization.Utf8Json.Dto;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaIntervalFormatter : IJsonFormatter<Interval>
    {
        public void Serialize(ref JsonWriter writer, Interval value, IJsonFormatterResolver formatterResolver)
        {
            if (value.HasStart && value.HasEnd)
            {
                formatterResolver.GetFormatterWithVerify<IntervalDto>().Serialize(ref writer, new IntervalDto
                {
                    Start = value.Start,
                    End = value.End
                }, formatterResolver);
                return;
            }

            if (value.HasStart)
            {
                formatterResolver.GetFormatterWithVerify<IntervalDtoStart>().Serialize(ref writer, new IntervalDtoStart
                {
                    Start = value.Start,
                }, formatterResolver);
                return;
            }

            if (value.HasEnd)
            {
                formatterResolver.GetFormatterWithVerify<IntervalDtoEnd>().Serialize(ref writer, new IntervalDtoEnd
                {
                    End = value.End,
                }, formatterResolver);
                return;
            }

            writer.WriteBeginObject();
            writer.WriteEndObject();
        }

        public Interval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var intervalDto = formatterResolver.GetFormatterWithVerify<IntervalDto>().Deserialize(ref reader, formatterResolver);
            return new Interval(intervalDto.Start, intervalDto.End);
        }
    }
}
