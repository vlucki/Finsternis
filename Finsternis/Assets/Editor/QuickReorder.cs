using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviour), true)]
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
        }
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}