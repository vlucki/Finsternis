using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
    Entity entity;
    GUIStyle style;
    static bool displayAttributes = false;
    private GenericMenu gm;

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
            } else
            {
                Debug.Log(Event.current.button);
            }
            Event.current.Use();
        }
        displayAttributes = EditorGUI.Foldout(foldRect, displayAttributes, "| ATTRIBUTES |");

        if (displayAttributes)
        {
            foreach (EntityAttribute attribute in entity.GetComponents<EntityAttribute>())
            {
                EditorGUILayout.BeginHorizontal();
                style.alignment = TextAnchor.MiddleRight;
                EditorGUILayout.LabelField(attribute.AttributeName, style, GUILayout.MaxWidth(40));
                attribute.SetValue(EditorGUILayout.IntField((int)attribute.Value, GUILayout.MaxWidth(40)));
                if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
                {
                    if (EditorUtility.DisplayDialog("Delete attribute", "This action cannot be undone.", "Proceed", "Cancel"))
                    {
                        DestroyImmediate(attribute);
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void AddAttribute()
    {
        entity.gameObject.AddComponent<RangedValueAttribute>();
    }
}