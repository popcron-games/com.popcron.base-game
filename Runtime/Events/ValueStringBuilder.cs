#nullable enable
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BaseGame
{
    public ref struct ValueStringBuilder
    {
        public readonly Span<char> buffer;
        private int position;

        public ValueStringBuilder(Span<char> buffer)
        {
            this.buffer = buffer;
            position = 0;
        }

        public readonly override string ToString()
        {
            return new string(buffer.Slice(0, position));
        }

        public readonly ReadOnlySpan<char> AsSpan()
        {
            return buffer.Slice(0, position);
        }

        public void Append(ReadOnlySpan<char> text)
        {
            text.CopyTo(buffer.Slice(position));
            position += text.Length;
        }

        public void Append<T>(T? value)
        {
            if (value is null) return;

            if (value is string str)
            {
                Append(str.AsSpan());
            }
            else
            {
                Append(GetString(value));
            }
        }

        public void Append(char c)
        {
            buffer[position++] = c;
        }

        public static ValueStringBuilder Create()
        {
            return new ValueStringBuilder(new char[256]);
        }

        public static ValueStringBuilder Create(int size)
        {
            return new ValueStringBuilder(new char[size]);
        }

        private static ReadOnlySpan<char> GetString<T>(T value)
        {
            if (value is not null)
            {
                ReadOnlySpan<char> text;
                ReadOnlySpan<char> prefix = ReadOnlySpan<char>.Empty;
                ReadOnlySpan<char> suffix = ReadOnlySpan<char>.Empty;
                if (value is FixedString fixedString)
                {
                    if (FixedString.TryGetString(fixedString.GetHashCode(), out string str))
                    {
                        prefix = "\"";
                        suffix = "\"";
                        text = str.AsSpan();
                    }
                    else
                    {
                        text = fixedString.GetHashCode().ToString().AsSpan();
                    }
                }
                else if (value is ID id)
                {
                    prefix = "`";
                    suffix = "`";
                    if (ID.TryGetString(id.GetHashCode(), out string str))
                    {
                        text = str.AsSpan();
                    }
                    else
                    {
                        text = id.GetHashCode().ToString().AsSpan();
                    }
                }
                else if (value is Type type)
                {
                    prefix = "<";
                    suffix = ">";
                    text = type.FullName.AsSpan();
                }
                else
                {
                    text = value.ToString() ?? ReadOnlySpan<char>.Empty;
                }

                bool hasPrefix = !prefix.IsEmpty;
                bool hasSuffix = !suffix.IsEmpty;
                if (hasPrefix && hasSuffix)
                {
                    Span<char> buffer = new char[prefix.Length + text.Length + suffix.Length];
                    prefix.CopyTo(buffer);
                    text.CopyTo(buffer.Slice(prefix.Length));
                    suffix.CopyTo(buffer.Slice(prefix.Length + text.Length));
                    return buffer;
                }
                else if (hasPrefix)
                {
                    Span<char> buffer = new char[prefix.Length + text.Length];
                    prefix.CopyTo(buffer);
                    text.CopyTo(buffer.Slice(prefix.Length));
                    return buffer;
                }
                else if (hasSuffix)
                {
                    Span<char> buffer = new char[text.Length + suffix.Length];
                    text.CopyTo(buffer);
                    suffix.CopyTo(buffer.Slice(text.Length));
                    return buffer;
                }
                else
                {
                    return text;
                }
            }
            else
            {
                return ReadOnlySpan<char>.Empty;
            }
        }

        public static ReadOnlySpan<char> Concat<T1, T2>(T1 arg1, T2 arg2) => Concat(GetString(arg1), GetString(arg2));
        public static ReadOnlySpan<char> Concat<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3) => Concat(GetString(arg1), GetString(arg2), GetString(arg3));
        public static ReadOnlySpan<char> Concat<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Concat(GetString(arg1), GetString(arg2), GetString(arg3), GetString(arg4));
        public static ReadOnlySpan<char> Concat<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => Concat(GetString(arg1), GetString(arg2), GetString(arg3), GetString(arg4), GetString(arg5));
        public static ReadOnlySpan<char> Concat<T1, T2, T3, T4, T5, T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => Concat(GetString(arg1), GetString(arg2), GetString(arg3), GetString(arg4), GetString(arg5), GetString(arg6));

        public static ReadOnlySpan<char> Concat(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            var builder = Create(str1.Length + str2.Length);
            builder.Append(str1);
            builder.Append(str2);
            return builder.AsSpan();
        }

        public static ReadOnlySpan<char> Concat(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2, ReadOnlySpan<char> str3)
        {
            var builder = Create(str1.Length + str2.Length + str3.Length);
            builder.Append(str1);
            builder.Append(str2);
            builder.Append(str3);
            return builder.AsSpan();
        }

        public static ReadOnlySpan<char> Concat(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2, ReadOnlySpan<char> str3, ReadOnlySpan<char> str4)
        {
            var builder = Create(str1.Length + str2.Length + str3.Length + str4.Length);
            builder.Append(str1);
            builder.Append(str2);
            builder.Append(str3);
            builder.Append(str4);
            return builder.AsSpan();
        }

        public static ReadOnlySpan<char> Concat(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2, ReadOnlySpan<char> str3, ReadOnlySpan<char> str4, ReadOnlySpan<char> str5)
        {
            var builder = Create(str1.Length + str2.Length + str3.Length + str4.Length + str5.Length);
            builder.Append(str1);
            builder.Append(str2);
            builder.Append(str3);
            builder.Append(str4);
            builder.Append(str5);
            return builder.AsSpan();
        }

        public static ReadOnlySpan<char> Concat(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2, ReadOnlySpan<char> str3, ReadOnlySpan<char> str4, ReadOnlySpan<char> str5, ReadOnlySpan<char> str6)
        {
            var builder = Create(str1.Length + str2.Length + str3.Length + str4.Length + str5.Length + str6.Length);
            builder.Append(str1);
            builder.Append(str2);
            builder.Append(str3);
            builder.Append(str4);
            builder.Append(str5);
            builder.Append(str6);
            return builder.AsSpan();
        }

        public static ReadOnlySpan<char> Format<T1>(ReadOnlySpan<char> text, T1 arg1) => Format(text, GetString(arg1));
        public static ReadOnlySpan<char> Format<T1, T2>(ReadOnlySpan<char> text, T1 arg1, T2 arg2) => Format(text, GetString(arg1), GetString(arg2));
        public static ReadOnlySpan<char> Format<T1, T2, T3>(ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3) => Format(text, GetString(arg1), GetString(arg2), GetString(arg3));
        public static ReadOnlySpan<char> Format<T1, T2, T3, T4>(ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => Format(text, GetString(arg1), GetString(arg2), GetString(arg3), GetString(arg4));

        public static ReadOnlySpan<char> Format(ReadOnlySpan<char> text, ReadOnlySpan<char> arg1)
        {
            return text.Replace("{0}", arg1);
        }

        public static ReadOnlySpan<char> Format(ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2)
        {
            Span<char> buffer = new char[text.Length + arg1.Length + arg2.Length];
            int index = 0;
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '{')
                {
                    if (text[i + 1] == '0')
                    {
                        arg1.CopyTo(buffer.Slice(index));
                        index += arg1.Length;
                        i += 3;
                        continue;
                    }
                    else if (text[i + 1] == '1')
                    {
                        arg2.CopyTo(buffer.Slice(index));
                        index += arg2.Length;
                        i += 3;
                        continue;
                    }
                }

                buffer[index++] = text[i++];
            }

            return buffer.Slice(0, index);
        }

        public static ReadOnlySpan<char> Format(ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3)
        {
            Span<char> buffer = new char[text.Length + arg1.Length + arg2.Length + arg3.Length];
            int index = 0;
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '{')
                {
                    if (text[i + 1] == '0')
                    {
                        arg1.CopyTo(buffer.Slice(index));
                        index += arg1.Length;
                        i += 3;
                        continue;
                    }
                    else if (text[i + 1] == '1')
                    {
                        arg2.CopyTo(buffer.Slice(index));
                        index += arg2.Length;
                        i += 3;
                        continue;
                    }
                    else if (text[i + 1] == '2')
                    {
                        arg3.CopyTo(buffer.Slice(index));
                        index += arg3.Length;
                        i += 3;
                        continue;
                    }
                }

                buffer[index++] = text[i++];
            }

            return buffer.Slice(0, index);
        }

        public static ReadOnlySpan<char> Format(ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> arg4)
        {
            Span<char> buffer = new char[text.Length + arg1.Length + arg2.Length + arg3.Length + arg4.Length];
            int index = 0;
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '{')
                {
                    if (text[i + 1] == '0')
                    {
                        arg1.CopyTo(buffer.Slice(index));
                        index += arg1.Length;
                        i += 3;
                        continue;
                    }
                    else if (text[i + 1] == '1')
                    {
                        arg2.CopyTo(buffer.Slice(index));
                        index += arg2.Length;
                        i += 3;
                        continue;
                    }
                    else if (text[i + 1] == '2')
                    {
                        arg3.CopyTo(buffer.Slice(index));
                        index += arg3.Length;
                        i += 3;
                        continue;
                    }
                    else if (text[i + 1] == '3')
                    {
                        arg4.CopyTo(buffer.Slice(index));
                        index += arg4.Length;
                        i += 3;
                        continue;
                    }
                }

                buffer[index++] = text[i++];
            }

            return buffer.Slice(0, index);
        }
    }
}