using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace DS.NodaTime.Serialization.Utf8Json
{
    public class Exceptions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowInvalidDateTimeFormat(ArraySegment<byte> str)
        {
            throw new InvalidOperationException("invalid datetime format. value:" +
                                                StringEncoding.UTF8.GetString(str.Array, str.Offset, str.Count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowInvalidTimeZone(ArraySegment<byte> str)
        {
            throw new InvalidOperationException("invalid timezone format. value:" +
                                                StringEncoding.UTF8.GetString(str.Array, str.Offset, str.Count));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowInvalidPeriodFormat(ArraySegment<byte> str)
        {
            throw new InvalidOperationException("invalid period format. value:" +
                                                StringEncoding.UTF8.GetString(str.Array, str.Offset, str.Count));
        }
    }
}
