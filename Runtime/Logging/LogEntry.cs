#nullable enable
using System;

namespace BaseGame
{
    public readonly struct LogEntry
    {
        public const byte ErrorType = 0;
        public const byte AssertType = 1;
        public const byte WarningType = 2;
        public const byte InfoType = 3;
        public const byte ExceptionType = 4;

        public readonly FixedString text;
        public readonly byte type;

        public LogEntry(FixedString text, byte type)
        {
            this.text = text;
            this.type = type;
        }

        public LogEntry(ReadOnlySpan<char> text, byte type)
        {
            this.text = text;
            this.type = type;
        }
    }
}