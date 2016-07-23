using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(AxesNameAttribute))]
public class CustomAxesDrawer : PropertyDrawer
{
    int selectedIndex = -1;
    GUIContent[] axes;

    public enum InputType
    {
        KeyOrMouseButton,
        MouseMovement,
        JoystickAxis,
    };

    void InitializeAxes(SerializedProperty property)
    {
        if (axes != null)
            return;

        var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];

        SerializedObject obj = new SerializedObject(inputManager);

        SerializedProperty axisArray = obj.FindProperty("m_Axes");

        if (axisArray.arraySize == 0)
        {
            Debug.LogWarning("No Axes found.");
            return;
        }
        axes = new GUIContent[axisArray.arraySize];

        for (int i = 0; i < axisArray.arraySize; ++i)
        {
            var axis = axisArray.GetArrayElementAtIndex(i);
            var name = axis.FindPropertyRelative("m_Name").stringValue;

            axes[i] = new GUIContent(name);
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType.Equals(SerializedPropertyType.String))
        {
            InitializeAxes(property);
            SelectAxis(property);

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, axes);
            if(EditorGUI.EndChangeCheck())
                property.stringValue = axes[selectedIndex].text;
        }
        else
        {
            EditorGUILayout.HelpBox("The field '" + label.text + "' must be a string.", MessageType.Error, true);
        }
    }

    private void SelectAxis(SerializedProperty property)
    {
        for (int i = 0; i < axes.Length; ++i)
            if (axes[i].text.Equals(property.stringValue))
            selectedIndex = i;
    }
}
