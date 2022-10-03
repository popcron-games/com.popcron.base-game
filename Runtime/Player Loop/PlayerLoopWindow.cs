#nullable enable

using UnityEditor;
using UnityEngine;

namespace BaseGame
{
    public class PlayerLoopWindow : EditorWindow
    {
        private bool isExpanded = true;
        private Vector2 scroll;
        private Rect lastViewRect;

        [MenuItem("Window/Player Loop")]
        public static void ShowWindow()
        {
            GetWindow<PlayerLoopWindow>("Player Loop");
        }

        private void OnGUI()
        {
            Repaint();
            Rect windowRect = new Rect(0, 0, position.width, position.height);
            scroll = GUI.BeginScrollView(windowRect, scroll, lastViewRect);

            lastViewRect = new Rect(0, 0, position.width - 20, 0);
            isExpanded = EditorGUILayout.Foldout(isExpanded, "Objects", true, EditorStyles.foldoutHeader);
            lastViewRect.height += EditorGUIUtility.singleLineHeight;
            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                Simulation? simulation = PlayerLoop.GetSimulation();
                if (simulation is not null)
                {
                    foreach (IComponent obj in simulation.GetAll())
                    {
                        EditorGUILayout.LabelField(obj?.ToString());
                        lastViewRect.height += EditorGUIUtility.singleLineHeight;
                        if (obj is IUpdateLoop)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("IUpdateLoop");
                            lastViewRect.height += EditorGUIUtility.singleLineHeight;
                            EditorGUI.indentLevel--;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
            GUI.EndScrollView();
        }
    }
}