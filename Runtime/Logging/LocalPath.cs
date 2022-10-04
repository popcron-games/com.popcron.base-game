#nullable enable
using System.IO;
using UnityEngine;

namespace BaseGame
{
    public static class LocalPath
    {
#if UNITY_EDITOR
        private const string PathPrefix = "Assets/RuntimeGame/";
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