using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Component), true)]
public class QuickReorder : Editor
{
    public override void OnInspectorGUI()
    {
        DrawQuickReorderHeader();
        base.OnInspectorGUI();
    }

    protected void DrawQuickReorderHeader()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("/\\", GUILayout.MaxWidth(30)))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(target as Component);
        }

        if (GUILayout.Button("\\/", GUILayout.MaxWidth(30)))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentDown(target as Component);
        }

        if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
        {
            DestroyImmediate(target);
            GUIUtility.ExitGUI();
            return;
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}