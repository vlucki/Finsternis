using UnityEngine;
using UnityEditor;
using System;
using Finsternis;

[CustomPropertyDrawer(typeof(EntityAttribute))]
public class EntityAttributeDrawer : PropertyDrawer
{
    private GUIContent emptyLabel = new GUIContent();
    private GUIContent basicLabel = new GUIContent("Attribute");
    private GUIStyle labelStyle;

    private GUIStyle LabelStyle
    {
        get
        {
            if (this.labelStyle == null)
            {

                this.labelStyle = new GUIStyle();
                this.labelStyle.alignment = TextAnchor.MiddleRight;
            }
            return this.labelStyle;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EntityAttribute attribute = property.objectReferenceValue as EntityAttribute;
        if (attribute)
        {
            var rect = position;
            rect.x += position.width * 0.1f;
            rect.width *= 0.3f;
            EditorGUI.LabelField(rect, GetAttributeLabel(attribute), LabelStyle);
            rect.x += position.width * 0.3f;
            rect.width = position.width * 0.6f;
            EditorGUI.PropertyField(rect, property, emptyLabel);
        }
        else
        {
            EditorGUI.PropertyField(position, property, basicLabel);
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