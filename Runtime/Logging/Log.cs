#nullable enable
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BaseGame
{
    [Serializable]
    public class Log : ILog
    {
        private static readonly List<Log> logs = new();

        private FixedString name;
        private List<LogEntry>? entries;

        public Log(ReadOnlySpan<char> name)
        {
            this.name = new FixedString(name);
            logs.Add(this);
        }

        [HideInCallstack]
        public void AddEntry<T>(T exception) where T : Exception
        {
            entries ??= new();
            entries.Add(new LogEntry(exception.Message, LogEntry.EntryType.Error));
        }

        [HideInCallstack]
        public void AddEntry(LogEntry entry)
        {
            entries ??= new();
            entries.Add(entry);

            //add to unity's console
            if (entry.Type == LogEntry.EntryType.Error)
            {
                Debug.LogError(entry.Text);
            }
            else if (entry.Type == LogEntry.EntryType.Warning)
            {
                Debug.LogWarning(entry.Text);
            }
            else
            {
                Debug.Log(entry.Text);
            }
        }

        public static async UniTask DumpAllLogs()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (Log log in logs)
            {
                string fileName = log.name;
                string folderPath = LocalPath.GetFullPath("Logs");
                string filePath = ValueStringBuilder.Format("{0}/{1}.txt", folderPath, fileName).ToString();

                //make sure the folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (log.entries is not null)
                {
                    using (var builder = ZString.CreateStringBuilder())
                    {
                        foreach (LogEntry entry in log.entries)
                        {
                            builder.AppendLine(entry.Text);
                        }
                        
                        await File.WriteAllTextAsync(filePath, builder.ToString()).AsUniTask();
                    }
                }
            }

            stopwatch.Stop();
            Debug.LogFormat("Dumped all logs in {0}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}