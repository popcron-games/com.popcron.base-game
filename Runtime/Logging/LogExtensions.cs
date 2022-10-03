#nullable enable
using System;
using UnityEngine;

namespace BaseGame
{
    public static class ILogExtensions
    {
        [HideInCallstack]
        public static void LogInfo<L>(this L log, ReadOnlySpan<char> message) where L : ILog
        {
            log.AddEntry(new LogEntry(message, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L, T1>(this L log, ReadOnlySpan<char> text, T1 arg1) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L, T1, T2>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L, T1, T2, T3>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> arg4) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3, arg4);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogInfoFormat<L, T1, T2, T3, T4>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3, arg4);
            log.AddEntry(new LogEntry(v, LogEntry.InfoType));
        }

        [HideInCallstack]
        public static void LogWarning<L>(this L log, ReadOnlySpan<char> message) where L : ILog
        {
            log.AddEntry(new LogEntry(message, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L, T1>(this L log, ReadOnlySpan<char> text, T1 arg1) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L, T1, T2>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L, T1, T2, T3>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> arg4) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3, arg4);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogWarningFormat<L, T1, T2, T3, T4>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3, arg4);
            log.AddEntry(new LogEntry(v, LogEntry.WarningType));
        }

        [HideInCallstack]
        public static void LogError<L>(this L log, ReadOnlySpan<char> message) where L : ILog
        {
            log.AddEntry(new LogEntry(message, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L, T1>(this L log, ReadOnlySpan<char> text, T1 arg1) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L, T1, T2>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L, T1, T2, T3>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L>(this L log, ReadOnlySpan<char> text, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> arg4) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3, arg4);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }

        [HideInCallstack]
        public static void LogErrorFormat<L, T1, T2, T3, T4>(this L log, ReadOnlySpan<char> text, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where L : ILog
        {
            ReadOnlySpan<char> v = ValueStringBuilder.Format(text, arg1, arg2, arg3, arg4);
            log.AddEntry(new LogEntry(v, LogEntry.ErrorType));
        }
    }
}