using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Component), true)]
public class QuickReorder : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.BeginHorizontal(style);
        
        if (GUILayout.Button("/\\"))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(target as Component);
        } else if (GUILayout.Button("\\/"))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentDown(target as Component);
        } else if (GUILayout.Button("X", GUILayout.MaxWidth(40)))
        {
            DestroyImmediate(target);
            GUIUtility.ExitGUI();
            return;
        }
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}