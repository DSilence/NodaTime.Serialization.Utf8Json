using NodaTime.Utility;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    internal static class TestHelper
    {

        internal static void AssertConversions<T>(T value, string expectedJson, IJsonFormatter formatter)
        {

            var actualJson = JsonSerializer.ToJsonString(value, CompositeResolver.Create(formatter));
            Assert.Equal(expectedJson, actualJson);

            var deserializedValue = JsonSerializer.Deserialize<T>(expectedJson, CompositeResolver.Create(formatter));
            Assert.Equal(value, deserializedValue);
        }

        internal static void AssertInvalidJson<T>(string json, IJsonFormatter formatter)
        {
            Assert.Throws<InvalidNodaDataException>(() => JsonSerializer.Deserialize<T>(json, CompositeResolver.Create(formatter)));
        }
    }
}
