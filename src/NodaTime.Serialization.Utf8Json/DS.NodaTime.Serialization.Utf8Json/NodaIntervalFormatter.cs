using System;
using NodaTime;
using Utf8Json;
using Utf8Json.Internal;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaIntervalFormatter : IJsonFormatter<Interval>
    {
        private readonly byte[] _startPropertyName;
        private readonly byte[] _endPropertyName;

        public NodaIntervalFormatter(Func<string, string> nameMutator)
        {
            _startPropertyName =
                JsonWriter.GetEncodedPropertyNameWithoutQuotation(nameMutator?.Invoke(nameof(Interval.Start)) ??
                                                  nameof(Interval.Start));
            _endPropertyName =
                JsonWriter.GetEncodedPropertyNameWithoutQuotation(nameMutator?.Invoke(nameof(Interval.End)) ?? nameof(Interval.End));
        }

        public void Serialize(ref JsonWriter writer, Interval value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginObject();
            var instantFormatter = formatterResolver.GetFormatter<Instant>();

            if (value.HasStart)
            {
                writer.WriteQuotation();
                writer.WriteRaw(_startPropertyName);
                writer.WriteQuotation();
                writer.WriteNameSeparator();
                instantFormatter.Serialize(ref writer, value.Start, formatterResolver);
                if (value.HasEnd)
                {
                    writer.WriteValueSeparator();
                }
            }

            if (value.HasEnd)
            {
                writer.WriteQuotation();
                writer.WriteRaw(_endPropertyName);
                writer.WriteQuotation();
                writer.WriteNameSeparator();
                instantFormatter.Serialize(ref writer, value.End, formatterResolver);
            }

            writer.WriteEndObject();
        }

        public Interval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            reader.ReadIsBeginObjectWithVerify();
            var instantFormatter = formatterResolver.GetFormatter<Instant>();
            Instant? startInstant = null;
            Instant? endInstant = null;
            var expectSeparator = false;
            while (!reader.ReadIsEndObject())
            {
                if (expectSeparator)
                {
                    reader.ReadIsValueSeparatorWithVerify();
                }
                var propertyName = reader.ReadPropertyNameSegmentRaw();

                if (ByteArrayComparer.Equals(propertyName.Array, propertyName.Offset, propertyName.Count,
                    _startPropertyName))
                {
                    startInstant = instantFormatter.Deserialize(ref reader, formatterResolver);
                    expectSeparator = true;
                }
                else if (ByteArrayComparer.Equals(propertyName.Array, propertyName.Offset, propertyName.Count,
                    _endPropertyName))
                {
                    endInstant = instantFormatter.Deserialize(ref reader, formatterResolver);
                    expectSeparator = true;
                }
                else
                {
                    break;
                }
            }

            return new Interval(startInstant, endInstant);
        }
    }
}
