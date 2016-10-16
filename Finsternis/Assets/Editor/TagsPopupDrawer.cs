using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TagSelectionAttribute))]
public class TagsPopupDrawer : PropertyDrawer
{
    int selectedIndex = -1;
    GUIContent[] listedTags;

    void Initialize(SerializedProperty property)
    {
        if (listedTags != null && listedTags.Length > 0)
            return;

        var tags = UnityEditorInternal.InternalEditorUtility.tags;
        listedTags = new GUIContent[tags.Length];

        for (int i = 0; i < listedTags.Length; i++)
        {
            listedTags[i] = new GUIContent(tags[i]);
            if (listedTags[i].text.Equals(property.stringValue))
                selectedIndex = i;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType.Equals(SerializedPropertyType.String))
        {
            Initialize(property);
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, listedTags);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = listedTags[selectedIndex].text;
        }
        else
        {
            EditorGUILayout.HelpBox("The field '" + label.text + "' must be a string.", MessageType.Error, true);
        }
    }
}