using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EntityAttribute), true)]
public class EntityAttributeEditor : QuickReorder
{
   static GUIStyle generalStyle;

    EntityAttribute attribute;

    protected override void OnEnable()
    {
        base.OnEnable();
        attribute = (EntityAttribute)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawAttribute(attribute);
    }

    public static void DrawAttribute(EntityAttribute attribute)
    {
        InitStyle();
        generalStyle.normal.textColor = Color.black;
        generalStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUI.BeginChangeCheck();

        generalStyle.border = new RectOffset(50, 50, 5, 5);
        generalStyle.alignment = TextAnchor.MiddleCenter;

        Rect r = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        if (attribute.LimitMaximum && attribute.LimitMinimum)
        {
            float progressValue = attribute.Max > 0 ? attribute.Value / attribute.Max : 1;
            EditorGUI.ProgressBar(r, progressValue, "");
        }

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.alignment = TextAnchor.MiddleRight;

        string attributeName = attribute.Name;
        bool hasName = !string.IsNullOrEmpty(attributeName);
        if (!hasName)
            attributeName = "name...";
        attributeName = EditorGUILayout.TextField(attributeName, style).
            TrimStart().Replace("[", "").Replace("]", "").Replace(".", "");

        string attributeAlias = attribute.Alias;
        bool hasAlias = !string.IsNullOrEmpty(attributeAlias);
        if (!hasAlias)
            attributeAlias = " ";
        style.alignment = TextAnchor.MiddleLeft;
        attributeAlias =
            EditorGUILayout.DelayedTextField("[" + attributeAlias + "]", style).
            Trim().Replace("[", "").Replace("]", "").Replace(".", "");

        if (string.IsNullOrEmpty(attributeAlias))
            attributeAlias = attribute.Alias;

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

        generalStyle.alignment = TextAnchor.MiddleRight;
        float? min = null;
        float? max = null;

        if (attribute.LimitMinimum)
            min = attribute.Min;
        if (attribute.LimitMaximum)
            max = attribute.Max;

        Rect minMaxRect = EditorGUILayout.BeginHorizontal(generalStyle);

        generalStyle.alignment = TextAnchor.MiddleRight;
        EditorGUILayout.LabelField("MIN", generalStyle, GUILayout.MaxWidth(30));

        if (min != null)
        {
            min = EditorGUILayout.FloatField((float)min, GUILayout.MaxWidth(30));

            if (GUI.Button(new Rect(minMaxRect.x + 70, minMaxRect.y + 2, 15, 15), "-", EditorStyles.miniButton))
            {
                min = null;
            }
            else
                GUILayout.Space(20);
        }
        else
        {
            if (GUI.Button(new Rect(minMaxRect.x + 35, minMaxRect.y + 2, 15, 15), "+", EditorStyles.miniButton))
                min = 0;
        }

        //GUILayout.FlexibleSpace();

        if (min != null && max != null)
        {
            float fmin = (float)min;
            float fmax = (float)max;
            EditorGUILayout.MinMaxSlider(ref fmin, ref fmax, 0, 999);
            min = fmin;
            max = fmax;
            GUILayout.Space(23);
        } else
        {
            GUILayout.FlexibleSpace();
        }

        if (max != null)
        {
            max = EditorGUILayout.FloatField((float)max, GUILayout.MaxWidth(30));
            if (GUI.Button(new Rect(minMaxRect.width - 67, minMaxRect.y + 2, 15, 15), "-", EditorStyles.miniButton))
                max = null;
        }
        else if (GUI.Button(new Rect(minMaxRect.width - 37, minMaxRect.y + 2, 15, 15), "+", EditorStyles.miniButton))
            max = 999;

        generalStyle.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.LabelField("MAX", generalStyle, GUILayout.MaxWidth(30));

        EditorGUILayout.EndHorizontal();

        float value = attribute.Value;
        if (min != null && max != null)
        {
            value = EditorGUILayout.Slider(GUIContent.none, attribute.Value, (float)min, (float)max);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            value = EditorGUILayout.FloatField(attribute.Value, GUILayout.MaxWidth(50));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(attribute, attribute.name + "[" + attribute.Name + " - " + attribute.Alias + "]");
            attribute.Name = attributeName;
            attribute.Alias = attributeAlias;
            attribute.SetValue(value);

            if (min != null)
            {
                attribute.SetMin((float)min);
            }
            else
            {
                attribute.LimitMinimum = false;
            }

            if (max != null)
            {
                attribute.SetMax((float)max);
            }
            else
            {
                attribute.LimitMaximum = false;
            }
        }
    }

    private static void InitStyle()
    {
        if (generalStyle == null)
        {
            generalStyle = new GUIStyle();
            generalStyle.margin = new RectOffset(2, 2, 2, 0);
            generalStyle.fontStyle = FontStyle.Bold;
        }
    }
}