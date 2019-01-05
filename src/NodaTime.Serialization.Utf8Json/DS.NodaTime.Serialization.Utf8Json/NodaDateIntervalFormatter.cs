using System;
using NodaTime;
using NodaTime.Utility;
using Utf8Json;
using Utf8Json.Internal;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaDateIntervalFormatter : IJsonFormatter<DateInterval>
    {
        private readonly byte[] _startPropertyName;
        private readonly byte[] _endPropertyName;

        public NodaDateIntervalFormatter(Func<string, string> nameMutator)
        {
            _startPropertyName =
                JsonWriter.GetEncodedPropertyNameWithoutQuotation(nameMutator?.Invoke(nameof(DateInterval.Start)) ??
                                                  nameof(DateInterval.Start));
            _endPropertyName =
                JsonWriter.GetEncodedPropertyNameWithoutQuotation(nameMutator?.Invoke(nameof(DateInterval.End)) ??
                                                  nameof(DateInterval.End));
        }

        public void Serialize(ref JsonWriter writer, DateInterval value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var dateIntervalFormatter = formatterResolver.GetFormatter<LocalDate>();

            writer.WriteBeginObject();

            writer.WriteQuotation();
            writer.WriteRaw(_startPropertyName);
            writer.WriteQuotation();
            writer.WriteNameSeparator();
            dateIntervalFormatter.Serialize(ref writer, value.Start, formatterResolver);
            writer.WriteValueSeparator();
            writer.WriteQuotation();
            writer.WriteRaw(_endPropertyName);
            writer.WriteQuotation();
            writer.WriteNameSeparator();
            dateIntervalFormatter.Serialize(ref writer, value.End, formatterResolver);

            writer.WriteEndObject();
        }

        public DateInterval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }

            var dateIntervalFormatter = formatterResolver.GetFormatter<LocalDate>();

            reader.ReadIsBeginObjectWithVerify();
            LocalDate? startLocalDate = null;
            LocalDate? endLocalDate = null;
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
                    startLocalDate = dateIntervalFormatter.Deserialize(ref reader, formatterResolver);
                    expectSeparator = true;
                }
                else if (ByteArrayComparer.Equals(propertyName.Array, propertyName.Offset, propertyName.Count,
                    _endPropertyName))
                {
                    endLocalDate = dateIntervalFormatter.Deserialize(ref reader, formatterResolver);
                    expectSeparator = true;
                }
                else
                {
                    break;
                }
            }

            if (!startLocalDate.HasValue)
            {
                throw new InvalidNodaDataException("Expected date interval; start date was missing.");
            }

            if (!endLocalDate.HasValue)
            {
                throw new InvalidNodaDataException("Expected date interval; end date was missing.");
            }

            return new DateInterval(startLocalDate.Value, endLocalDate.Value);
        }
    }
}
