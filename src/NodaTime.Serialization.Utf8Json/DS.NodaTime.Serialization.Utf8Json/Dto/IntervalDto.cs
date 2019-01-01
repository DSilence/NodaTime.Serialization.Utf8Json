using NodaTime;

namespace DS.NodaTime.Serialization.Utf8Json.Dto
{
    public struct IntervalDtoStart
    {
        public Instant Start { get; set; }
    }

    public struct IntervalDtoEnd
    {
        public Instant End { get; set; }
    }

    public struct IntervalDto
    {
        public Instant? Start { get; set; }

        public Instant? End { get; set; }
    }
}
