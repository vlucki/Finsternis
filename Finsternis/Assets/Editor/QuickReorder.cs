using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(UnityEngine.Component), true, isFallback = true)]
[CanEditMultipleObjects]
public class QuickReorder : Editor
{
    private GUILayoutOption[] buttonOptions;
    private GUIStyle leftButton;
    private GUIStyle middleButton;
    private GUIStyle rightButton;

    public bool initialized;

    protected virtual void OnEnable()
    {
        initialized = false;
    }


    private void Init()
    {
        buttonOptions = new GUILayoutOption[] { GUILayout.MaxWidth(25), GUILayout.MaxHeight(18) };
        leftButton = MakeButtonStyle(EditorStyles.miniButtonLeft);
        middleButton = MakeButtonStyle(EditorStyles.miniButtonMid);
        rightButton = MakeButtonStyle(EditorStyles.miniButtonRight);

        initialized = true;
    }

    public GUIStyle MakeButtonStyle(GUIStyle baseStyle)
    {
        GUIStyle buttonStyle = new GUIStyle(baseStyle);
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.padding = new RectOffset(1, 1, 1, 1);
        return buttonStyle;
    }

    public override void OnInspectorGUI()
    {
        DrawQuickReorderHeader();

        base.OnInspectorGUI();
    }

    protected void DrawQuickReorderHeader()
    {
        if(!initialized)
        {
            Init();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(77), GUILayout.MaxHeight(20));
        if (GUILayout.Button("/\\", leftButton, buttonOptions))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(target as Component);
            GUIUtility.ExitGUI();
            return;
        }

        if (GUILayout.Button("\\/", middleButton, buttonOptions))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentDown(target as Component);
            GUIUtility.ExitGUI();
            return;
        }

        if (GUILayout.Button("X", rightButton, buttonOptions))
        {
            if (EditorUtility.DisplayDialog("Delete component?", "This action cannot be undone.", "Proceed", "Cancel"))
            {
                DestroyImmediate(target, true);
                GUIUtility.ExitGUI();
                return;
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
}