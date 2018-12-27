// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Serialization.Utf8Json.Dto;
using Utf8Json;

namespace NodaTime.Serialization.Utf8Json
{
    /// <summary>
    /// Json.NET converter for <see cref="Interval"/> using a compound representation. The start and
    /// end aspects of the interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="Instant"/> converter for the serializer provided.
    /// </summary>   
    internal sealed class NodaIntervalFormatter : IJsonFormatter<Interval>
    {
        public void Serialize(ref JsonWriter writer, Interval value, IJsonFormatterResolver formatterResolver)
        {
            if (value.HasStart && value.HasEnd)
            {
                formatterResolver.GetFormatter<IntervalDto>().Serialize(ref writer, new IntervalDto
                {
                    Start = value.Start,
                    End = value.End
                }, formatterResolver);
                return;
            }

            if (value.HasStart)
            {
                formatterResolver.GetFormatter<IntervalDtoStart>().Serialize(ref writer, new IntervalDtoStart
                {
                    Start = value.Start,
                }, formatterResolver);
                return;
            }

            if (value.HasEnd)
            {
                formatterResolver.GetFormatter<IntervalDtoEnd>().Serialize(ref writer, new IntervalDtoEnd
                {
                    End = value.Start,
                }, formatterResolver);
            }
        }

        public Interval Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var intervalDto = formatterResolver.GetFormatter<IntervalDto>().Deserialize(ref reader, formatterResolver);
            return new Interval(intervalDto.Start, intervalDto.End);
        }
    }
}
