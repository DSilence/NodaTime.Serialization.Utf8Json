[![Build Status](https://dev.azure.com/DzmitrySafarau0213/NodaTime.Serialization.Utf8Json/_apis/build/status/DSilence.NodaTime.Serialization.Utf8Json?branchName=develop)](https://dev.azure.com/DzmitrySafarau0213/NodaTime.Serialization.Utf8Json/_build/latest?definitionId=1?branchName=develop)
![](https://img.shields.io/azure-devops/tests/DzmitrySafarau0213/NodaTime.Serialization.Utf8Json/1.svg)
![](https://img.shields.io/nuget/dt/DS.NodaTime.Serialization.Utf8Json.svg)

# NodaTime.Serialization.Utf8Json
A port of NodaTime.Serialization.JsonNet to utilize Utf8Json instead

## Sample Usage

```csharp
using DS.NodaTime.Serialization.Utf8Json;
using NodaTime;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
           CompositeResolver.RegisterAndSetAsDefault(
                new NodaTimeResolver(DateTimeZoneProviders.Tzdb, NameHandling.CamelCase, true, true, true),
                StandardResolver.ExcludeNullCamelCase);
            var json = JsonSerializer.ToJsonString(new LocalDate(2019, 1, 1);
        }
    }
}
```