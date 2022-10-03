#nullable enable

using System;
using Unity.Netcode;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class NetworkVariableString : NetworkVariableBase
    {
        [SerializeField]
        private string? value;

        public NetworkVariableString() : base()
        {

        }

        public NetworkVariableString(ReadOnlySpan<char> value)
        {
            this.value = value.ToString();
        }

        public string? Value
        {
            get => value;
            set => this.value = value;
        }

        public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
        {
            ReadField(reader);
        }

        public override unsafe void ReadField(FastBufferReader reader)
        {
            reader.ReadByte(out byte length);

            Span<byte> bytes = stackalloc byte[length];
            fixed (byte* ptr = bytes)
            {
                reader.ReadBytes(ptr, length);
            }
        }

        public override void WriteDelta(FastBufferWriter writer)
        {
            WriteField(writer);
        }

        public override void WriteField(FastBufferWriter writer)
        {
            ReadOnlySpan<char> value = !string.IsNullOrEmpty(this.value) ? this.value.AsSpan() : ReadOnlySpan<char>.Empty;
            byte length = (byte)value.Length;
            
            writer.WriteByte(length);
            for (int i = 0; i < length; i++)
            {
                writer.WriteByte((byte)value[i]);
            }
        }
    }
}