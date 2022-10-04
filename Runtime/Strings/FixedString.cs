#nullable enable
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace BaseGame
{
    public readonly struct FixedString : IEquatable<FixedString>, INetworkSerializeByMemcpy
    {
        public static readonly FixedString Empty = new FixedString(0);

        private static readonly Dictionary<int, string> strings = new();

        public readonly int hashCode;

        public readonly bool IsEmpty => hashCode == 0;

        public FixedString(ReadOnlySpan<char> text)
        {
            this.hashCode = text.GetSpanHashCode();
            if (!text.IsEmpty)
            {
                strings[this.hashCode] = text.ToString();
            }
        }

        public FixedString(int hashCode)
        {
            this.hashCode = hashCode;
        }

        public readonly bool Equals(FixedString other) => hashCode == other.hashCode;
        public override readonly bool Equals(object? obj) => obj is FixedString other && Equals(other);
        public override readonly int GetHashCode() => hashCode;
        
        public override readonly string ToString()
        {
            if (strings.TryGetValue(hashCode, out string text))
            {
                return text;
            }

            return hashCode.ToString();
        }

        public ReadOnlySpan<char> AsSpan()
        {
            if (strings.TryGetValue(hashCode, out string text))
            {
                return text.AsSpan();
            }
            else
            {
                return ReadOnlySpan<char>.Empty;
            }
        }

        public static bool TryGetString(int hashCode, out string name) => strings.TryGetValue(hashCode, out name);

        public static int Parse(ReadOnlySpan<char> text)
        {
            return text.GetSpanHashCode();
        }

        public static implicit operator FixedString(string text) => new(text);
        public static implicit operator FixedString(ReadOnlySpan<char> text) => new(text);
        public static implicit operator string(FixedString text) => text.ToString();
        public static implicit operator ReadOnlySpan<char>(FixedString text) => text.AsSpan();
        public static bool operator ==(FixedString left, FixedString right) => left.Equals(right);
        public static bool operator !=(FixedString left, FixedString right) => !left.Equals(right);
        public static bool operator ==(FixedString left, ReadOnlySpan<char> right) => left.hashCode == right.GetSpanHashCode();
        public static bool operator !=(FixedString left, ReadOnlySpan<char> right) => left.hashCode != right.GetSpanHashCode();
        public static bool operator ==(ReadOnlySpan<char> left, FixedString right) => left.GetSpanHashCode() == right.hashCode;
        public static bool operator !=(ReadOnlySpan<char> left, FixedString right) => left.GetSpanHashCode() != right.hashCode;
    }
}
