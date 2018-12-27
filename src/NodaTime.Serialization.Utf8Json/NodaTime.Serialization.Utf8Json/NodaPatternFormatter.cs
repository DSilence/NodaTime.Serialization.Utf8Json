using System;
using NodaTime.Text;
using NodaTime.Utility;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    public class NodaPatternFormatter<T>: IJsonFormatter<T>
    {
        private readonly IPattern<T> _pattern;
        private readonly Action<T> _validator;

        public NodaPatternFormatter(IPattern<T> pattern, Action<T> validator = null)
        {
            _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
            _validator = validator;
        }

        public void Serialize(ref JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            _validator?.Invoke(value);
            writer.WriteString(_pattern.Format(value));
        }

        public T Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var text = reader.ReadString();
            return _pattern.Parse(text).Value;
        }
    }
}