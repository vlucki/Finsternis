using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(PopupAttribute))]
public class EasyPopupDrawer : PropertyDrawer
{
    private int selectedIndex;
    GUIContent[] items;

    private void SelectAxis(SerializedProperty property, PopupAttribute attribute)
    {
        for (selectedIndex = items.Length - 1; selectedIndex >= 0; selectedIndex--)
        {
            switch (attribute.popupType) {
                case PopupAttribute.PopUpType.STRING:
                    if (items[selectedIndex].text.Equals(property.stringValue))
                        return;
                    break;
                case PopupAttribute.PopUpType.INT:
                    if (items[selectedIndex].text.Equals(property.intValue.ToString()))
                        return;
                    break;
                case PopupAttribute.PopUpType.FLOAT:
                    if (items[selectedIndex].text.Equals(property.floatValue.ToString()))
                        return;
                    break;
            }
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        PopupAttribute attr = attribute as PopupAttribute;
        InitializeItems(attr);
        SelectAxis(property, attr);
        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUI.Popup(position, label, selectedIndex, items);
        if (EditorGUI.EndChangeCheck())
        {
            SetPropertyValue(property, attr);
        }
    }

    private void SetPropertyValue(SerializedProperty property, PopupAttribute attr)
    {
        switch (attr.popupType)
        {
            case PopupAttribute.PopUpType.STRING:
                property.stringValue = items[selectedIndex].text;
                break;
            case PopupAttribute.PopUpType.INT:
                property.intValue = (int)attr.Items[selectedIndex];
                break;
            case PopupAttribute.PopUpType.FLOAT:
                property.floatValue = (float)attr.Items[selectedIndex];
                break;
        }
    }

    private void InitializeItems(PopupAttribute attr)
    {
        items = new GUIContent[attr.Items.Count];
        for (int i = 0; i < items.Length; i++)
            items[i] = new GUIContent(attr.Items[i].ToString());
    }
}
#endif
