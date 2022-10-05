#nullable enable
using System;

namespace BaseGame
{
    public interface ILog
    {
        void AddEntry<T>(T exception) where T : Exception;
        void AddEntry(LogEntry entry);
    }
}