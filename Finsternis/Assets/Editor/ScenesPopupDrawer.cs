using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneSelectionAttribute))]
public class ScenesPopupDrawer : PropertyDrawer
{
    int selectedIndex = -1;
    GUIContent[] listedScenes;

    void Initialize(SerializedProperty property)
    {
        if (listedScenes != null && listedScenes.Length > 0)
            return;

        var scenes = EditorBuildSettings.scenes;
        var selectableScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        SceneSelectionAttribute attr = (SceneSelectionAttribute)attribute;

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].enabled || attr.allowDisabledScenes)
                selectableScenes.Add(scenes[i]);
        }
        listedScenes = new GUIContent[selectableScenes.Count];

        for (int i = 0; i < listedScenes.Length; i++)
        {
            var path = selectableScenes[i].path;
            int lastSeparator = path.LastIndexOf("/") + 1;
            var sceneName = path.Substring(lastSeparator, path.LastIndexOf(".") - lastSeparator);
            listedScenes[i] = new GUIContent(sceneName, selectableScenes[i].enabled ? "Enabled" : "Disabled");
            if (listedScenes[i].text.Equals(property.stringValue))
                selectedIndex = i;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType.Equals(SerializedPropertyType.String))
        {
            Initialize(property);
            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(position, label, selectedIndex, listedScenes);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = listedScenes[selectedIndex].text;
        }
        else
        {
            EditorGUILayout.HelpBox("The field '" + label.text + "' must be a string.", MessageType.Error, true);
        }
    }
}