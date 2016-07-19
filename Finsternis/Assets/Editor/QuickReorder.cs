using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(UnityEngine.Component), true, isFallback = true)]
[CanEditMultipleObjects]
public class QuickReorder : Editor
{
    private GUILayoutOption[] _buttonOptions;
    private GUIStyle _leftButton;
    private GUIStyle _middleButton;
    private GUIStyle _rightButton;

    private bool _initialized;

    private GUIContent _moveUpButtonContent;
    private GUIContent _moveDownButtonContent;
    private GUIContent _deleteButtonContent;
    private GUIStyle _boxStyle;

    protected virtual void OnEnable()
    {
        //sometimes, Unity throws a NullReferenceException if Init() is called here
        //because EditorStyles are not yet initialized (apparently), so we delay the initialization
        _initialized = false;
    }


    private void Init()
    {
        _buttonOptions = new GUILayoutOption[] { GUILayout.MaxWidth(25), GUILayout.MaxHeight(18) };
        _leftButton = MakeButtonStyle(EditorStyles.miniButtonLeft);
        _middleButton = MakeButtonStyle(EditorStyles.miniButtonMid);
        _rightButton = MakeButtonStyle(EditorStyles.miniButtonRight);
        _moveUpButtonContent = new GUIContent("/\\", "Move Component Up");
        _moveDownButtonContent = new GUIContent("\\/", "Move Component Down");
        _deleteButtonContent = new GUIContent("X", "Delete Component");
        _boxStyle = new GUIStyle(EditorStyles.helpBox);
        _boxStyle.alignment = TextAnchor.UpperCenter;
        _boxStyle.fontStyle = FontStyle.Bold;
        _initialized = true;
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
        if(!_initialized)
        {
            Init();
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical("Quick Reorder", _boxStyle);
        GUILayout.Space(15);
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(77), GUILayout.MaxHeight(20));

        if (GUILayout.Button(_moveUpButtonContent, _leftButton, _buttonOptions))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(target as Component);
            GUIUtility.ExitGUI();
            return;
        }

        if (GUILayout.Button(_moveDownButtonContent, _middleButton, _buttonOptions))
        {
            UnityEditorInternal.ComponentUtility.MoveComponentDown(target as Component);
            GUIUtility.ExitGUI();
            return;
        }

        if (GUILayout.Button(_deleteButtonContent, _rightButton, _buttonOptions))
        {
            if (EditorUtility.DisplayDialog("Delete component?", "This action cannot be undone.", "Proceed", "Cancel"))
            {
                DestroyImmediate(target, true);
                GUIUtility.ExitGUI();
                return;
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
}