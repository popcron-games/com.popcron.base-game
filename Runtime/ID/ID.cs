#nullable enable
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace BaseGame
{
    public readonly struct ID : IEquatable<ID>, INetworkSerializeByMemcpy
    {
        public static readonly ID Empty = new ID(0);

        private static readonly Dictionary<int, string> strings = new();

        private readonly int hashCode;

        public readonly bool IsEmpty => hashCode == 0;

        public ID(ReadOnlySpan<char> text)
        {
            this.hashCode = Parse(text);
            if (!text.IsEmpty)
            {
                strings[this.hashCode] = text.ToString();
            }
        }

        public ID(int id, ReadOnlySpan<char> text = default)
        {
            this.hashCode = id;
            if (!text.IsEmpty)
            {
                strings[this.hashCode] = text.ToString();
            }
        }

        public readonly bool Equals(ID other) => hashCode == other.hashCode;
        public readonly bool Equals(ReadOnlySpan<char> other) => hashCode == Parse(other);
        public readonly bool Equals(FixedString other)
        {
            if (FixedString.TryGetString(other.hashCode, out string text))
            {
                return hashCode == Parse(text);
            }
            else
            {
                return other.hashCode == hashCode;
            }
        }

        public override readonly bool Equals(object? obj) => obj is ID other && Equals(other);
        public override readonly int GetHashCode() => hashCode;

        public override readonly string ToString()
        {
            if (strings.TryGetValue(hashCode, out string text))
            {
                return text;
            }

            return hashCode.ToString();
        }

        public static bool TryGetString(int hashCode, out string name) => strings.TryGetValue(hashCode, out name);

        public static int Parse(ReadOnlySpan<char> text)
        {
            unchecked
            {
                //identical to GetSpanHashCode but lowercase only,
                //effectively making all IDs case insensitive
                int length = text.Length;
                int hashCode = length;
                for (int i = 0; i < length; i++)
                {
                    hashCode = (hashCode * 397) ^ char.ToLowerInvariant(text[i]);
                }

                return hashCode;
            }
        }

        public static ID CreateRandom()
        {
            Random rng = new();
            return new ID(rng.Next());
        }

        public static implicit operator ID(string text) => new(text);
        public static implicit operator ID(ReadOnlySpan<char> text) => new(text);
        public static implicit operator ID(int id) => new(id);
        public static implicit operator int(ID id) => id.hashCode;
        public static implicit operator ID(ulong id) => new((int)id);
        public static implicit operator ulong(ID id) => (ulong)id.hashCode;
        public static bool operator ==(ID left, ID right) => left.Equals(right);
        public static bool operator !=(ID left, ID right) => !left.Equals(right);
        public static bool operator ==(ID left, ReadOnlySpan<char> right) => left.Equals(right);
        public static bool operator !=(ID left, ReadOnlySpan<char> right) => !left.Equals(right);
        public static bool operator ==(ReadOnlySpan<char> left, ID right) => right.Equals(left);
        public static bool operator !=(ReadOnlySpan<char> left, ID right) => !right.Equals(left);
        public static bool operator ==(ID left, FixedString right) => left.Equals(right);
        public static bool operator !=(ID left, FixedString right) => !left.Equals(right);
        public static bool operator ==(FixedString left, ID right) => right.Equals(left);
        public static bool operator !=(FixedString left, ID right) => !right.Equals(left);
    }
}
