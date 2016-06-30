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
        displayAttributes = EditorGUI.Foldout(foldRect, displayAttributes, "| ATTRIBUTES |");

        if (displayAttributes)
        {
            style.alignment = TextAnchor.MiddleRight;
            scrollPos = GUILayout.BeginScrollView(scrollPos, style, GUILayout.MinWidth(75), GUILayout.MinHeight(140));
            foreach (EntityAttribute attribute in entity.GetComponents<EntityAttribute>())
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(260));
                style.alignment = TextAnchor.MiddleRight;

                attribute.AttributeName = EditorGUILayout.TextField(attribute.AttributeName, style, GUILayout.MaxWidth(60));
                if (attribute is RangedValueAttribute)
                {
                    RangedValueAttribute rvAttribute = attribute as RangedValueAttribute;
                    attribute.SetValue(EditorGUILayout.IntSlider((int)rvAttribute.Value, (int)rvAttribute.Min, (int)rvAttribute.Max, GUILayout.MaxWidth(60)));
                }
                else
                {
                    attribute.SetValue(EditorGUILayout.IntField((int)attribute.Value, GUILayout.MaxWidth(60)));
                }


                if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                {
                    if (EditorUtility.DisplayDialog("Delete attribute", "This action cannot be undone.", "Proceed", "Cancel"))
                    {
                        DestroyImmediate(attribute);
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();

            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Add Attribute"))
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