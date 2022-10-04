#nullable enable
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace BaseGame
{
    public readonly struct FixedString : IEquatable<FixedString>, INetworkSerializeByMemcpy
    {
        public static readonly FixedString Empty = new FixedString(0, 0);

        private static readonly Dictionary<int, string> strings = new();

        public readonly int hashCode;
        public readonly byte length;

        public FixedString(string text)
        {
            this.hashCode = text.GetSpanHashCode();
            this.length = (byte)text.Length;
            if (!strings.ContainsKey(hashCode))
            {
                strings.Add(hashCode, text);
            }
        }

        public FixedString(int hashCode, byte length = 0)
        {
            this.hashCode = hashCode;
            this.length = length;
        }

        public FixedString(ReadOnlySpan<char> text)
        {
            this.hashCode = text.GetSpanHashCode();
            this.length = (byte)text.Length;
            if (!strings.ContainsKey(hashCode))
            {
                strings.Add(hashCode, text.ToString());
            }
        }

        public readonly bool Equals(FixedString other) => hashCode == other.hashCode;
        public override readonly bool Equals(object? obj) => obj is FixedString other && Equals(other);
        public override readonly int GetHashCode() => hashCode;

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
        
        public override readonly string ToString() => ToString(hashCode);

        public static string ToString(int hashCode)
        {
            if (strings.ContainsKey(hashCode))
            {
                return strings[hashCode];
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool HasString(int hashCode)
        {
            return strings.ContainsKey(hashCode);
        }

        public static void SetString(int hashCode, string text)
        {
            strings[hashCode] = text;
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
