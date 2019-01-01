using NodaTime;
using NodaTime.Text;
using NodaTime.Utility;
using Utf8Json;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class NodaIsoIntervalFormatter : IJsonFormatter<Interval>
    {
        public void Serialize(ref JsonWriter writer, Interval value, IJsonFormatterResolver formatterResolver)
        {
            var pattern = InstantPattern.ExtendedIso;
            var text = (value.HasStart ? pattern.Format(value.Start) : "") + "/" +
                       (value.HasEnd ? pattern.Format(value.End) : "");
            writer.WriteString(text);
        }

        public Interval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var text = reader.ReadString();
            var slash = text.IndexOf('/');
            if (slash == -1)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted interval; slash was missing.");
            }
            var startText = text.Substring(0, slash);
            var endText = text.Substring(slash + 1);
            var pattern = InstantPattern.ExtendedIso;
            var start = startText == "" ? (Instant?) null : pattern.Parse(startText).Value;
            var end = endText == "" ? (Instant?) null : pattern.Parse(endText).Value;

            return new Interval(start, end);
        }
    }
}
