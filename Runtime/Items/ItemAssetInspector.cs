#nullable enable
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    [CustomEditor(typeof(ItemAsset), false)]
    public class ItemAssetInspector : Editor
    {
        private ItemAsset itemAsset = null!;
        private SerializedProperty idProperty = null!;
        private SerializedProperty itemProperty = null!;

        private void OnEnable()
        {
            itemAsset = (ItemAsset)target;
            idProperty = serializedObject.FindProperty("id");
            itemProperty = serializedObject.FindProperty("item");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            //draw a serialize reference button
            Rect position = EditorGUILayout.GetControlRect();
            var typeRestrictions = new List<Func<Type, bool>>();
            itemProperty.DrawSelectionButtonForManagedReference(position, typeRestrictions);
            EditorGUI.LabelField(position, "Item type");
            EditorGUILayout.PropertyField(idProperty);

            //find the category type by searching the base type up until it reaches Item
            if (itemAsset.PrefabItem is not null)
            {
                Type categoryType = itemAsset.PrefabItem.GetType();
                string category = string.Empty;
                while (true) //safe
                {
                    if (categoryType.BaseType is null)
                    {
                        break;
                    }
                    else if (categoryType.BaseType == typeof(Item))
                    {
                        break;
                    }
                    else if (categoryType.GetCustomAttribute<ItemCategoryAttribute>() is ItemCategoryAttribute itemCategory)
                    {
                        category = itemCategory.Category;
                        break;
                    }
                    
                    categoryType = categoryType.BaseType;
                    category = categoryType.Name;
                }

                EditorGUILayout.LabelField("Category", category, EditorStyles.boldLabel);
            }

            //show all properties
            SerializedProperty iterator = serializedObject.GetIterator();
            SerializedObject? itemSerializedObject = null;
            int depth = iterator.depth;
            if (iterator.NextVisible(true))
            {
                do
                {
                    if (itemSerializedObject is not null)
                    {
                        //id is first field, and is already drawn before
                        if (iterator.name == "id")
                        {
                            depth = iterator.depth;
                            continue;
                        }
                        else if (iterator.name == "prefabId")
                        {
                            //this is the prefab id so it has no prefab of its own
                            continue;
                        }

                        EditorGUI.indentLevel = iterator.depth - depth;
                        EditorGUILayout.PropertyField(iterator);
                    }
                    else if (iterator.name == "item")
                    {
                        itemSerializedObject = iterator.serializedObject;
                    }
                }
                while (iterator.NextVisible(itemSerializedObject is not null));
            }

            serializedObject.ApplyModifiedProperties();

            if (target is IValidate validate)
            {
                if (validate.Validate())
                {
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif