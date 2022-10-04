#nullable enable
using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace BaseGame
{
    public readonly struct ID : IEquatable<ID>, INetworkSerializeByMemcpy
    {
        public static Dictionary<int, string> idStrings = new();

        public static readonly ID Empty = new ID(0);

        private readonly int id;

        public ID(string text)
        {
            this.id = Parse(text);
            if (text.Length > 0)
            {
                idStrings[id] = text;
            }
        }

        public ID(ReadOnlySpan<char> text)
        {
            this.id = Parse(text);
            if (text.Length > 0)
            {
                idStrings[id] = text.ToString();
            }
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
            if (idStrings.TryGetValue(id, out string? text))
            {
                return text;
            }
            else
            {
                return id.ToString("X");
            }
        }

        public static int Parse(ReadOnlySpan<char> text)
        {
            unchecked
            {
                //try to read as hex first
                IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture;
                if (int.TryParse(text, System.Globalization.NumberStyles.HexNumber, provider, out int hex))
                {
                    return hex;
                }

                //read case insensitive hash code
                int length = text.Length;
                int hash = length;
                for (int i = 0; i < length; i++)
                {
                    hash = (hash * 31) ^ char.ToLowerInvariant(text[i]);
                }

                if (!idStrings.ContainsKey(hash))
                {
                    idStrings[hash] = text.ToString();
                }

                return hash;
            }
        }

        public static ID CreateRandom()
        {
            Random rng = new();
            return new ID(rng.Next());
        }

        public static bool operator ==(ID left, ID right) => left.Equals(right);
        public static bool operator !=(ID left, ID right) => !left.Equals(right);
        public static bool operator ==(ID left, ReadOnlySpan<char> right) => left.Equals(right);
        public static bool operator !=(ID left, ReadOnlySpan<char> right) => !left.Equals(right);
        public static bool operator ==(ReadOnlySpan<char> left, ID right) => right.Equals(left);
        public static bool operator !=(ReadOnlySpan<char> left, ID right) => !right.Equals(left);
    }
}
