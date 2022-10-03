#define STORE_STRING
#nullable enable
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace BaseGame
{
    public readonly struct ID : IEquatable<ID>, INetworkSerializeByMemcpy
    {
#if STORE_STRING
        public static readonly Dictionary<int, string> idToName = new();
#endif

        public static readonly ID Empty = new ID(0);

        private readonly int id;

        public ID(ReadOnlySpan<char> text)
        {
            this.id = Parse(text);
#if STORE_STRING
            idToName[this.id] = text.ToString();
#endif
        }

        public ID(int id)
        {
            this.id = id;
        }

        public readonly bool Equals(ID other) => id == other.id;
        public readonly bool Equals(ReadOnlySpan<char> other) => id == Parse(other);
        public override readonly bool Equals(object? obj) => obj is ID other && Equals(other);
        public override readonly int GetHashCode() => id;

        public override readonly string ToString()
        {
#if STORE_STRING
            if (idToName.TryGetValue(id, out var name))
            {
                return name;
            }
#endif

            return id.ToString("X");
        }

        public static int Parse(ReadOnlySpan<char> text)
        {
            unchecked
            {
                IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture;
                if (int.TryParse(text, System.Globalization.NumberStyles.HexNumber, provider, out int hex))
                {
                    return hex;
                }

                int length = text.Length;
                int hash = length;
                for (int i = 0; i < length; i++)
                {
                    hash = (hash * 31) ^ char.ToLowerInvariant(text[i]);
                }

                return hash;
            }
        }

        public static ID CreateRandom()
        {
            Random rng = new();
            return rng.Next();
        }

        public static implicit operator ID(string text) => new(text);
        public static implicit operator ID(ReadOnlySpan<char> text) => new(text);
        public static implicit operator ID(int id) => new(id);
        public static implicit operator int(ID id) => id.id;
        public static implicit operator ID(ulong id) => new((int)id);
        public static implicit operator ulong(ID id) => (ulong)id.id;
        public static bool operator ==(ID left, ID right) => left.Equals(right);
        public static bool operator !=(ID left, ID right) => !left.Equals(right);
        public static bool operator ==(ID left, ReadOnlySpan<char> right) => left.Equals(right);
        public static bool operator !=(ID left, ReadOnlySpan<char> right) => !left.Equals(right);
        public static bool operator ==(ReadOnlySpan<char> left, ID right) => right.Equals(left);
        public static bool operator !=(ReadOnlySpan<char> left, ID right) => !right.Equals(left);
    }
}
