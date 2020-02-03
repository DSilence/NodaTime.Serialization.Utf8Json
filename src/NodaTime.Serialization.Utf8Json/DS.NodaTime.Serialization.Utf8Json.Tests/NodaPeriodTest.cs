using DS.NodaTime.Serialization.Utf8Json;
using Utf8Json;
using Utf8Json.Resolvers;
using Xunit;

namespace NodaTime.Serialization.Utf8Json.Tests
{
    public class NodaPeriodTest
    {
        private static readonly IJsonFormatterResolver Resolver = CompositeResolver.Create(new IJsonFormatter[]
        {
            NodaFormatters.NormalizingIsoPeriodFormatter
        }, new[] { StandardResolver.Default });

        [Fact]
        public void Days_InObject()
        {
            var testObj = new PeriodTestObj
            {
                Str = "abcdefghijklmnopqrstuvwxyz",
                Period = Period.FromDays(3)
            };
            var json = JsonSerializer.ToJsonString(testObj, Resolver);

            var deserializedObj = JsonSerializer.Deserialize<PeriodTestObj>(json, Resolver);

            Assert.Equal(testObj.Period, deserializedObj.Period);
        }

        [Fact]
        public void Months_InObject()
        {
            var testObj = new PeriodTestObj
            {
                Str = "abcdefghijklmnopqrstuvwxyz",
                Period = Period.FromMonths(6)
            };
            var json = JsonSerializer.ToJsonString(testObj, Resolver);

            var deserializedObj = JsonSerializer.Deserialize<PeriodTestObj>(json, Resolver);

            Assert.Equal(testObj.Period, deserializedObj.Period);
        }

        [Fact]
        public void Years_InObject()
        {
            var testObj = new PeriodTestObj
            {
                Str = "abcdefghijklmnopqrstuvwxyz",
                Period = Period.FromYears(10)
            };
            var json = JsonSerializer.ToJsonString(testObj, Resolver);

            var deserializedObj = JsonSerializer.Deserialize<PeriodTestObj>(json, Resolver);

            Assert.Equal(testObj.Period, deserializedObj.Period);
        }

        [Fact]
        public void Hours_InObject()
        {
            var testObj = new PeriodTestObj
            {
                Str = "abcdefghijklmnopqrstuvwxyz",
                Period = Period.FromHours(8)
            };
            var json = JsonSerializer.ToJsonString(testObj, Resolver);

            var deserializedObj = JsonSerializer.Deserialize<PeriodTestObj>(json, Resolver);

            Assert.Equal(testObj.Period, deserializedObj.Period);
        }
    }

    public class PeriodTestObj
    {
        public string Str { get; set; }

        public Period Period { get; set; }
    }
}
