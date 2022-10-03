#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class SerializedID
    {
        [SerializeField]
        private string? text;

        public SerializedID() { }

        public SerializedID(ReadOnlySpan<char> text)
        {
            this.text = text.ToString();
        }

        public ID ID => new ID(text);

        public static implicit operator SerializedID(ReadOnlySpan<char> text) => new(text);
        public static implicit operator SerializedID(ID id) => new(id.ToString());
    }
}
