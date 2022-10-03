#nullable enable
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;
using UnityEditorInternal;
using System.Reflection;

namespace BaseGame
{
    [CustomEditor(typeof(TypeCacheSettings))]
    public class TypeCacheSettingsInspector : Editor
    {
        private static List<Assembly>? relevantAssemblies;

        private static List<Assembly> LoadRelevantAssemblies()
        {
            List<Assembly> relevantAssemblies = new();
            var unityAssemblies = CompilationPipeline.GetAssemblies();
            string projectFolder = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            foreach (var unityAssembly in unityAssemblies)
            {
                //find the assembly that matches the unity assembly
                Assembly? assemblyToLoad = null;
                foreach (var assembly in EditorAssemblyCache.assemblies)
                {
                    var name = assembly.GetName().Name;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    IEnumerable<Type> types = TypeCacheSettings.GetTypesToLoad(assembly) ?? Array.Empty<Type>();
                    foreach (Type type in types)
                    {
                        string relativeToProjectPath = assembly.Location.Substring(projectFolder.Length).Replace('\\', '/');
                        if (relativeToProjectPath == unityAssembly.outputPath)
                        {
                            assemblyToLoad = assembly;
                        }

                        break;
                    }
                }

                if (assemblyToLoad is not null)
                {
                    relevantAssemblies.Add(assemblyToLoad);
                }
            }

            //sort by name
            relevantAssemblies.Sort((a, b) => string.Compare(a.GetName().Name, b.GetName().Name, StringComparison.Ordinal));
            return relevantAssemblies;
        }

        public override void OnInspectorGUI()
        {
            if (relevantAssemblies is null)
            {
                relevantAssemblies = LoadRelevantAssemblies();
            }

            SerializedProperty typeNamesToLoadFrom = serializedObject.FindProperty("typeNamesToLoadFrom");
            EditorGUILayout.PropertyField(typeNamesToLoadFrom, true);

            //remove types that cant be found anymore and duplicates
            HashSet<string> typeNames = new();
            for (int i = typeNamesToLoadFrom.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty element = typeNamesToLoadFrom.GetArrayElementAtIndex(i);
                string typeName = element.stringValue;
                if (typeNames.Contains(typeName))
                {
                    typeNamesToLoadFrom.DeleteArrayElementAtIndex(i);
                }
                else
                {
                    typeNames.Add(typeName);
                    Type? type = Type.GetType(typeName);
                    if (type is null)
                    {
                        typeNamesToLoadFrom.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            string[] asmGuids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");
            foreach (string asmGuid in asmGuids)
            {
                string asmPath = AssetDatabase.GUIDToAssetPath(asmGuid);
                AssemblyDefinitionAsset asm = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmPath);
            }

            if (relevantAssemblies.Count == 0)
            {
                EditorGUILayout.HelpBox("No assemblies found", MessageType.Error);
                return;
            }

            foreach (Assembly assembly in relevantAssemblies)
            {
                string? typeName = TypeCacheSettings.GetTypeNameToLoad(assembly);
                if (typeName is not null)
                {
                    bool isLoaded = Contains(typeName);
                    bool shouldLoad = EditorGUILayout.ToggleLeft(typeName, isLoaded);
                    if (shouldLoad && !isLoaded)
                    {
                        typeNamesToLoadFrom.InsertArrayElementAtIndex(typeNamesToLoadFrom.arraySize);
                        typeNamesToLoadFrom.GetArrayElementAtIndex(typeNamesToLoadFrom.arraySize - 1).stringValue = typeName;
                        serializedObject.ApplyModifiedProperties();
                    }
                    else if (!shouldLoad && isLoaded)
                    {
                        int index = IndexOf(typeName);
                        typeNamesToLoadFrom.DeleteArrayElementAtIndex(index);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            int IndexOf(string typeName)
            {
                for (int i = 0; i < typeNamesToLoadFrom.arraySize; i++)
                {
                    SerializedProperty element = typeNamesToLoadFrom.GetArrayElementAtIndex(i);
                    if (element.stringValue == typeName)
                    {
                        return i;
                    }
                }

                return -1;
            }

            bool Contains(string typeName)
            {
                for (int i = 0; i < typeNamesToLoadFrom.arraySize; i++)
                {
                    SerializedProperty element = typeNamesToLoadFrom.GetArrayElementAtIndex(i);
                    if (element.stringValue == typeName)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
#endif