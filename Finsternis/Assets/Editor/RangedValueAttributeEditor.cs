using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(RangedValueAttribute))]
public class RangedValueAttributeEditor : Editor
{
    MonoScript script;
    RangedValueAttribute tgt;
    GUIStyle style;
    SerializedProperty attributeName;
    SerializedProperty val;
    SerializedProperty changeEvt;

    void OnEnable()
    {
        tgt = target as RangedValueAttribute;
        script = MonoScript.FromMonoBehaviour(tgt);
        style = new GUIStyle();
        style.margin = new RectOffset(2, 2, 2, 0);
        style.fontStyle = FontStyle.Bold;
        attributeName = serializedObject.FindProperty("attributeName");
        val = serializedObject.FindProperty("value");
        changeEvt = serializedObject.FindProperty("onValueChanged");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        script = EditorGUILayout.ObjectField("Script:", script, typeof(MonoScript), false) as MonoScript;

        style.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.BeginHorizontal(style);
        EditorGUILayout.LabelField("NAME", style, GUILayout.MaxWidth(40));
        EditorGUILayout.DelayedTextField(attributeName, GUIContent.none, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();

        float min = tgt.Min;
        float max = tgt.Max;

        EditorGUILayout.BeginHorizontal(style);

        EditorGUILayout.LabelField("MIN", style, GUILayout.MaxWidth(30));

        min = EditorGUILayout.FloatField(min, GUILayout.MaxWidth(40));

        EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 100);

        max = EditorGUILayout.FloatField(max, GUILayout.MaxWidth(40));

        style.alignment = TextAnchor.MiddleRight;

        EditorGUILayout.LabelField("MAX", style, GUILayout.MaxWidth(30));

        EditorGUILayout.EndHorizontal();

        tgt.SetMin(min);
        tgt.SetMax(max);

        style.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.BeginHorizontal(style);
        EditorGUILayout.LabelField("Current", style, GUILayout.MaxWidth(50));
        EditorGUILayout.Slider(val, min, max, GUIContent.none);
        EditorGUILayout.EndHorizontal();

        ProgressBar(val.floatValue, "Current " + attributeName.stringValue.ToUpper() + " (" + val.floatValue.ToString("F2") + "/" + tgt.Max.ToString("F2") + ")");

        EditorGUILayout.PropertyField(changeEvt);

        serializedObject.ApplyModifiedProperties();
    }

    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, style);
        EditorGUI.ProgressBar(rect, value / tgt.Max, label);
        EditorGUILayout.Space();
    }
}