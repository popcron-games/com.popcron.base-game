#nullable enable
using System;
using System.Collections.Generic;
using BaseGame;

namespace UnityEngine
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private readonly static string searchFilter;
        private static T? instance;

        public static T Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = RecoverInstance();
                }

                return instance;
            }
        }

        static ScriptableSingleton()
        {
            searchFilter = ValueStringBuilder.Concat("t:", typeof(T).Name).ToString();
        }

        protected ScriptableSingleton()
        {
            instance = this as T;
        }

        private void OnEnable()
        {
            instance = this as T;
            Validator.PerformValidation(this);
        }

        private void OnValidate()
        {
            Validator.PerformValidation(this);
        }

        protected static string GetFilePath()
        {
            return ValueStringBuilder.Format("Assets/{0}.asset", typeof(T).Name).ToString();
        }

        private static T? GetFromAssetDatabase()
        {
#if UNITY_EDITOR
            //try to find this instance in the player settings always included assets
            Object[] alwaysIncluded = UnityEditor.PlayerSettings.GetPreloadedAssets();
            foreach (Object asset in alwaysIncluded)
            {
                if (asset is T instance)
                {
                    return instance;
                }
            }

            //try to find this instance in from asset db
            string[] guids = UnityEditor.AssetDatabase.FindAssets(searchFilter);
            string path;
            foreach (string guid in guids)
            {
                path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                T instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                if (instance != null)
                {
                    return instance;
                }
            }
#endif
            return null;
        }

        private static T RecoverInstance()
        {
#if UNITY_EDITOR
            T? instance = GetFromAssetDatabase();
            if (instance is null)
            {
                //no instance found in asset database, create a new one
                string path = GetFilePath();
                instance = CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(instance, path);
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();

                //add this instance to the player settings always included assets
                Object[] alwaysIncluded = UnityEditor.PlayerSettings.GetPreloadedAssets();
                List<Object> newAlwaysIncluded = new List<Object>(alwaysIncluded);
                newAlwaysIncluded.Add(instance);
                UnityEditor.PlayerSettings.SetPreloadedAssets(newAlwaysIncluded.ToArray());

                Debug.LogFormat("Created new instance of {0} to be used as singleton at {1}", typeof(T).Name, path);
            }

            return instance;
#endif
            //a case that should not happen!
            throw new NotImplementedException();
        }
    }
}