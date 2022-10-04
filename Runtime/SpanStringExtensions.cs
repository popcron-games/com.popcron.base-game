#nullable enable
using System;

public static class SpanStringExtensions
{
    public static int GetSpanHashCode(this string textString)
    {
        int hashCode = 0;
        int length = textString.Length;
        for (int i = 0; i < length; i++)
        {
            hashCode = (hashCode * 397) ^ textString[i];
        }

        return hashCode;
    }

    public static int GetSpanHashCode(this ReadOnlySpan<char> textSpan)
    {
        int hashCode = 0;
        int length = textSpan.Length;
        for (int i = 0; i < length; i++)
        {
            hashCode = (hashCode * 397) ^ textSpan[i];
        }

        return hashCode;
    }

    public static int IndexOf(this ReadOnlySpan<char> text, char character, int startIndex)
    {
        int length = text.Length;
        for (int i = startIndex; i < length; i++)
        {
            if (text[i] == character)
            {
                return i;
            }
        }

        return -1;
    }

    public static ReadOnlySpan<char> Replace(this ReadOnlySpan<char> text, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
    {
        int difference = newValue.Length - oldValue.Length;
        int size = text.Length + difference + 32;
        Span<char> buffer = new char[size];

        int index = 0;
        int position = 0;
        int length = text.Length;
        while (index < length)
        {
            int nextIndex = text.IndexOf(oldValue[0], index);
            if (nextIndex == -1)
            {
                text.Slice(index).CopyTo(buffer.Slice(position));
                position += text.Length - index;
                break;
            }

            text.Slice(index, nextIndex - index).CopyTo(buffer.Slice(position));
            position += nextIndex - index;
            newValue.CopyTo(buffer.Slice(position));
            position += newValue.Length;
            index = nextIndex + oldValue.Length;
        }

        return buffer.Slice(0, position);
    }
}
