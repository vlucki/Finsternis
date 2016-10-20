using UnityEngine;
using UnityEditor;
using System;
using Finsternis;

[CustomPropertyDrawer(typeof(EntityAttribute))]
public class EntityAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EntityAttribute attribute = property.objectReferenceValue as EntityAttribute;
        if (attribute)
        {
            EditorGUI.PropertyField(position, property, GetAttributeLabel(attribute));
        }
        else
        {
            EditorGUI.PropertyField(position, property);
        }
    }

    private GUIContent GetAttributeLabel(EntityAttribute attribute)
    {
        GUIContent label = new GUIContent(attribute.Alias + " (" + attribute.Value);
        if (attribute.LimitMaximum)
            label.text += "/" + attribute.Max.ToString();
        label.text += ")";
        return label;
    }
}