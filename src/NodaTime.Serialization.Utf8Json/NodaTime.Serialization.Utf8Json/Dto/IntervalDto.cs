namespace NodaTime.Serialization.Utf8Json.Dto
{
    internal struct IntervalDtoStart
    {
        public Instant Start { get; set; }
    }

    internal struct IntervalDtoEnd
    {
        public Instant End { get; set; }
    }

    internal struct IntervalDto
    {
        public Instant? Start { get; set; }

        public Instant? End { get; set; }
    }
}