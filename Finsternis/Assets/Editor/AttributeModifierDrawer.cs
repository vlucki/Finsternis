using UnityEngine;
using UnityEditor;
using System;
using Finsternis;

[CustomPropertyDrawer(typeof(AttributeModifier))]
public class AttributeModifierDrawer : PropertyDrawer
{
    GUIContent emptyLabel = new GUIContent("");


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property != null)
        {

            position.height = 18;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("affectedAttribute"));
            position.y += 23;

            var rect = position;
            rect.width = 75f;
            EditorGUI.LabelField(rect, new GUIContent("Modifier"));

            rect.x += rect.width;
            rect.width = 57;
            rect.y -= 5;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("modifierType"), emptyLabel);

            rect.y += 5;
            rect.x += rect.width;
            rect.width = position.width - 132;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("valueChange"), emptyLabel);
            rect.x += rect.width;
            rect.width *= 3f;

        }
        else
        {
            EditorGUI.PropertyField(position, property);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 60f;
    }

}