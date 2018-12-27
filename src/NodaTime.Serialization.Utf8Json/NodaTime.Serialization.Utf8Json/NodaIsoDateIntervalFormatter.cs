using NodaTime.Text;
using NodaTime.Utility;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaIsoDateIntervalFormatter : IJsonFormatter<DateInterval>
    {
        public void Serialize(ref JsonWriter writer, DateInterval value, IJsonFormatterResolver formatterResolver)
        {
            var pattern = LocalDatePattern.Iso;
            var text = pattern.Format(value.Start) + "/" + pattern.Format(value.End);
            writer.WriteString(text);
        }

        public DateInterval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            string text = reader.ReadString();
            int slash = text.IndexOf('/');
            if (slash == -1)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date interval; slash was missing.");
            }

            string startText = text.Substring(0, slash);
            if (startText == "")
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date interval; start date was missing.");
            }

            string endText = text.Substring(slash + 1);
            if (endText == "")
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted date interval; end date was missing.");
            }

            var pattern = LocalDatePattern.Iso;
            var start = pattern.Parse(startText).Value;
            var end = pattern.Parse(endText).Value;

            return new DateInterval(start, end);
        }
    }
}