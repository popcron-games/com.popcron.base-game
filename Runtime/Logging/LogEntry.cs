#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    [Serializable]
    public class LogEntry
    {
        [SerializeField] private string text;
        [SerializeField] private EntryType type;

        public string Text => text;
        public EntryType Type => type;

        public LogEntry(ReadOnlySpan<char> text, EntryType type)
        {
            this.text = text.ToString();
            this.type = type;
        }

        public enum EntryType : byte
        {
            Info,
            Warning,
            Error
        }
    }
}