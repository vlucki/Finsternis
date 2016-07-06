using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : QuickReorder
{
    Entity entity;
    GUIStyle style;
    static bool displayAttributes = false;
    private GenericMenu gm;
    private Vector2 scrollPos;

    void OnEnable()
    {
        entity = target as Entity;
        style = new GUIStyle();
        style.margin = new RectOffset(2, 2, 2, 0);
        style.fontStyle = FontStyle.Bold;
        gm = new GenericMenu();
        gm.AddItem(new GUIContent("Add Attribute"), false, AddAttribute);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.GetControlRect(true, 16f, EditorStyles.foldout);
        Rect foldRect = GUILayoutUtility.GetLastRect();
        if (Event.current.type == EventType.MouseUp && foldRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 0)
            {
                displayAttributes = !displayAttributes;
                GUI.changed = true;
            } else if(Event.current.button == 1)
            {
                gm.ShowAsContext();
            }
            Event.current.Use();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        displayAttributes = EditorGUI.Foldout(foldRect, displayAttributes, "| ATTRIBUTES |");

        if (displayAttributes)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(240));
            foreach (EntityAttribute attribute in entity.GetComponents<EntityAttribute>())
            {
                style.border = new RectOffset(50, 50, 5, 5);
                EditorGUILayout.BeginVertical(style);
                style.alignment = TextAnchor.MiddleCenter;
                attribute.AttributeName = EditorGUILayout.TextField(attribute.AttributeName, style);
                EditorGUILayout.BeginHorizontal();
                style.alignment = TextAnchor.MiddleRight;

                if (attribute is RangedValueAttribute)
                {
                    RangedValueAttribute rvAttribute = attribute as RangedValueAttribute;
                    float min = rvAttribute.Min;
                    float max = rvAttribute.Max;

                    EditorGUILayout.BeginHorizontal(style);

                    EditorGUILayout.LabelField("MIN", style, GUILayout.MaxWidth(30));

                    min = EditorGUILayout.FloatField(min, GUILayout.MaxWidth(40));

                    EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 100);

                    max = EditorGUILayout.FloatField(max, GUILayout.MaxWidth(40));

                    style.alignment = TextAnchor.MiddleRight;

                    EditorGUILayout.LabelField("MAX", style, GUILayout.MaxWidth(30));

                    EditorGUILayout.EndHorizontal();

                    rvAttribute.SetMin(min);
                    rvAttribute.SetMax(max);
                }
                else
                {
                    attribute.SetValue(EditorGUILayout.IntField((int)attribute.Value, GUILayout.MaxWidth(40)));
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("REMOVE ATTRIBUTE", GUILayout.MaxWidth(150)))
                {
                    if (EditorUtility.DisplayDialog("Delete attribute", "This action cannot be undone.", "Proceed", "Cancel"))
                    {
                        DestroyImmediate(attribute);
                        GUIUtility.ExitGUI();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            if (GUILayout.Button("ADD ATTRIBUTE"))
            {
                AddAttribute();
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void AddAttribute()
    {
        entity.gameObject.AddComponent<RangedValueAttribute>();
    }
}