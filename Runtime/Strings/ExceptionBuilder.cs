#nullable enable

using System;

namespace BaseGame
{
    public class ExceptionBuilder
    {
        public static Exception Format(ReadOnlySpan<char> message, ReadOnlySpan<char> arg1)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1);
            return new Exception(v.ToString());
        }

        public static Exception Format(ReadOnlySpan<char> message, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1, arg2);
            return new Exception(v.ToString());
        }

        public static Exception Format(ReadOnlySpan<char> message, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1, arg2, arg3);
            return new Exception(v.ToString());
        }

        public static Exception Format(ReadOnlySpan<char> message, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> arg4)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1, arg2, arg3, arg4);
            return new Exception(v.ToString());
        }

        public static Exception Format<T1>(ReadOnlySpan<char> message, T1 arg1)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1);
            return new Exception(v.ToString());
        }

        public static Exception Format<T1, T2>(ReadOnlySpan<char> message, T1 arg1, T2 arg2)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1, arg2);
            return new Exception(v.ToString());
        }

        public static Exception Format<T1, T2, T3>(ReadOnlySpan<char> message, T1 arg1, T2 arg2, T3 arg3)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1, arg2, arg3);
            return new Exception(v.ToString());
        }

        public static Exception Format<T1, T2, T3, T4>(ReadOnlySpan<char> message, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(message, arg1, arg2, arg3, arg4);
            return new Exception(v.ToString());
        }
    }
}
