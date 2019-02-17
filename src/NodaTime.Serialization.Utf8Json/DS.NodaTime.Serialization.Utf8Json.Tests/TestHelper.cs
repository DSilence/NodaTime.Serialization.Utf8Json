using NodaTime.Utility;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    internal static class TestHelper
    {
        internal static void AssertConversions<T>(T value, string expectedJson, params IJsonFormatterResolver[] formatterResolvers)
        {
            var actualJson = JsonSerializer.ToJsonString(value, CompositeResolver.Create(formatterResolvers));
            Assert.Equal(expectedJson, actualJson);

            var deserializedValue = JsonSerializer.Deserialize<T>(expectedJson, CompositeResolver.Create(formatterResolvers));
            Assert.Equal(value, deserializedValue);
        }

        internal static void AssertConversions<T>(T value, string expectedJson, params IJsonFormatter[] formatter)
        {

            var actualJson = JsonSerializer.ToJsonString(value, CompositeResolver.Create(formatter));
            Assert.Equal(expectedJson, actualJson);

            var deserializedValue = JsonSerializer.Deserialize<T>(expectedJson, CompositeResolver.Create(formatter));
            Assert.Equal(value, deserializedValue);
        }

        internal static void AssertInvalidJson<T>(string json, IJsonFormatterResolver formatter)
        {
            Assert.Throws<InvalidNodaDataException>(() => JsonSerializer.Deserialize<T>(json, formatter));
        }
    }
}
