using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Character))]
[CanEditMultipleObjects]
public class CharacterEditor : Editor
{

    GUIStyle alignCenter = new GUIStyle();
    GUIStyle alignLeft = new GUIStyle();
    GUIStyle alignRight = new GUIStyle();

    RangedValueAttribute hp;
    RangedValueAttribute mp;
    RangedValueAttribute dmg;
    RangedValueAttribute def;

    public void OnEnable()
    {
        alignCenter.alignment = TextAnchor.MiddleCenter;

        alignLeft.alignment = TextAnchor.MiddleLeft;

        alignRight.alignment = TextAnchor.MiddleRight;

        hp = ((Character)target).health;
        mp = ((Character)target).mana;
        dmg = ((Character)target).damage;
        def = ((Character)target).defense;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Undo.RecordObject(target, "character");

        EditorGUILayout.BeginVertical();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Character)target), typeof(MonoScript), true);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.LabelField("BASE ATTRIBUTES", EditorStyles.boldLabel);
        DrawMinMaxValue(hp);
        DrawMinMaxValue(mp);
        DrawRangedValue(dmg);
        DrawRangedValue(def);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRangedValue(RangedValueAttribute attribute)
    {
        EditorGUILayout.BeginHorizontal(alignCenter);
        
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 1;
        EditorGUILayout.LabelField(attribute.Name, EditorStyles.boldLabel);
        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 10;
        attribute.SetValue(EditorGUILayout.IntSlider((int)attribute.Value, (int)attribute.Min, (int)attribute.Max));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawMinMaxValue(RangedValueAttribute attribute)
    {
        EditorGUILayout.BeginHorizontal(alignCenter);

        Vector2 newValue = new Vector2(attribute.Value, attribute.Max);
        EditorGUILayout.Space();
        EditorGUIUtility.labelWidth = 1;
        EditorGUILayout.LabelField(attribute.Name, EditorStyles.boldLabel);

        EditorGUIUtility.labelWidth = 20;
        EditorGUILayout.LabelField("(current) " + newValue.x, alignRight);

        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 100;
        EditorGUILayout.MinMaxSlider(ref (newValue.x), ref newValue.y, 0, 100);

        EditorGUIUtility.labelWidth = 20;
        EditorGUILayout.LabelField(newValue.y + " (maximum)", alignLeft);
        EditorGUILayout.Space();

        attribute.SetValue((int)newValue.x);
        attribute.SetMax((int)newValue.y);

        EditorGUILayout.EndHorizontal();
    }
}