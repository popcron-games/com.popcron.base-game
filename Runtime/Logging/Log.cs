#nullable enable
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        public void AddEntry(LogEntry entry)
        {
            entries ??= new();
            entries.Add(entry);

            //add to unity's console
            if (entry.type == LogEntry.ErrorType || entry.type == LogEntry.ExceptionType || entry.type == LogEntry.AssertType)
            {
                Debug.LogError(entry.text);
            }
            else if (entry.type == LogEntry.WarningType)
            {
                Debug.LogWarning(entry.text);
            }
            else
            {
                Debug.Log(entry.text);
            }
        }

        public static async UniTask DumpAllLogs()
        {
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
                            builder.AppendLine(entry.text);
                        }
                        
                        await File.WriteAllTextAsync(filePath, builder.ToString()).AsUniTask();
                    }
                }
            }
        }
    }

    public static class LocalPath
    {
#if UNITY_EDITOR
        private const string PathPrefix = "Assets/Game~/";
#else
        private const string PathPrefix = "";
#endif

        public static string GetFullPath(string path)
        {
            string parentFolder = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(parentFolder, PathPrefix, path);
        }
    }
}