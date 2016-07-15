using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
    Entity entity;
    GUIStyle foldoutStyle;
    GUIStyle scrollViewStyle;
    GUIStyle generalStyle;
    static bool displayAttributes = false;

    EntityAttribute[] attributes;
    bool[] attributesVisibility;
    
    private Vector2 scrollPos;
    private GUIStyle removeAttributeButtonStyle;

    protected void OnEnable()
    {
        entity = target as Entity;

        PopulateAttributesList();

        generalStyle = new GUIStyle();
        generalStyle.margin = new RectOffset(2, 2, 2, 0);
        generalStyle.fontStyle = FontStyle.Bold;

        foldoutStyle = new GUIStyle(EditorStyles.foldout);
        foldoutStyle.fontStyle = FontStyle.Bold;

        scrollViewStyle = new GUIStyle(EditorStyles.helpBox);
        scrollViewStyle.padding = new RectOffset(5, 5, 5, 5);

        CreateRemoveAttributeStyle();
    }

    private void PopulateAttributesList()
    {
        EntityAttribute[] currentAttributes = entity.GetComponents<EntityAttribute>();
        if (attributes == null)
        {
            attributes = currentAttributes;
            attributesVisibility = new bool[attributes.Length];
        }
        int difference = currentAttributes.Length - attributes.Length;
        if (difference != 0)
        {
            bool[] newVisibility = new bool[currentAttributes.Length];
            for (int i = 0; i < currentAttributes.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if (currentAttributes[i].Equals(attributes[j]))
                        newVisibility[i] = attributesVisibility[j];
                }
            }
            attributes = currentAttributes;
            attributesVisibility = newVisibility;
            if (difference > 0)
                attributesVisibility[attributesVisibility.Length - 1] = true; //make last added attribute visible

        }
    }

    private void CreateRemoveAttributeStyle()
    {
        removeAttributeButtonStyle = new GUIStyle(EditorStyles.miniButton);
        removeAttributeButtonStyle.fontStyle = FontStyle.Bold;
        removeAttributeButtonStyle.fontSize = 10;

        Texture2D bg = removeAttributeButtonStyle.normal.background;

        removeAttributeButtonStyle.hover.textColor = new Color(1f, 0.1f, 0.1f);
        removeAttributeButtonStyle.hover.background = bg;

        removeAttributeButtonStyle.active.textColor = new Color(0.5f, 0.1f, 0.1f);
        removeAttributeButtonStyle.active.background = bg;

        removeAttributeButtonStyle.focused.textColor = new Color(1f, 0.1f, 0.5f);
        removeAttributeButtonStyle.focused.background = bg;
    }

    void DrawAttributes()
    {
        for (int index = 0; index < attributes.Length; index++)
        {
            EntityAttribute attribute = attributes[index];

            EditorGUI.BeginChangeCheck();

            Rect r = EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (attribute.LimitMaximum && attribute.LimitMinimum)
            {
                float progressValue = attribute.Max > 0 ? attribute.Value / attribute.Max : 1;
                EditorGUI.ProgressBar(r, progressValue, "");
            }

            EditorGUILayout.BeginHorizontal();


            string label = "(" + attribute.Value;
            if (attribute.LimitMaximum && attribute.LimitMinimum)
                label += " / " + attribute.Max;
            label += ")";
            EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel, GUILayout.Width(30), GUILayout.ExpandWidth(true), GUILayout.MaxWidth(100));


            GUILayout.FlexibleSpace();

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.MiddleRight;

            string attributeName = attribute.Name;
            bool hasName = !string.IsNullOrEmpty(attributeName);
            if (!hasName)
                attributeName = "name...";
            attributeName = EditorGUILayout.TextField(attributeName, style).
                TrimStart().Replace("[", "").Replace("]", "").Replace(".", "");

            string attributeAlias = attribute.Alias;
            bool hasAlias = !string.IsNullOrEmpty(attributeAlias);
            if (!hasAlias)
                attributeAlias = " ";
            style.alignment = TextAnchor.MiddleLeft;
            attributeAlias =
                EditorGUILayout.DelayedTextField("[" + attributeAlias + "]", style).
                Trim().Replace("[", "").Replace("]", "").Replace(".", "");

            if (string.IsNullOrEmpty(attributeAlias))
                attributeAlias = attribute.Alias;

            GUILayout.FlexibleSpace();

            DrawButton(!attributesVisibility[index] ? "\\/" : "/\\", index, ToggleAttributeDisplay);
            DrawButton("X", index, RemoveAttribute, removeAttributeButtonStyle);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            float? min = null;
            float? max = null;

            if (attribute.LimitMinimum)
                min = attribute.Min;
            if (attribute.LimitMaximum)
                max = attribute.Max;

            float value = attribute.Value;

            if (attributesVisibility[index])
            {
                generalStyle.border = new RectOffset(50, 50, 5, 5);

                Rect minMaxRect = EditorGUILayout.BeginHorizontal(generalStyle);

                generalStyle.alignment = TextAnchor.MiddleRight;
                EditorGUILayout.LabelField("MIN", generalStyle, GUILayout.MaxWidth(30));

                if (min != null)
                {
                    min = EditorGUILayout.FloatField((float)min, GUILayout.MaxWidth(30));

                    if (GUI.Button(new Rect(minMaxRect.x + 70, minMaxRect.y + 2, 15, 15), "-", EditorStyles.miniButton))
                    {
                        min = null;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(minMaxRect.x + 35, minMaxRect.y + 2, 15, 15), "+", EditorStyles.miniButton))
                        min = 0;
                }

                if (min != null && max != null)
                {
                    GUILayout.Space(15);
                    float fmin = (float)min;
                    float fmax = (float)max;
                    EditorGUILayout.MinMaxSlider(ref fmin, ref fmax, 0, 999);
                    min = fmin;
                    max = fmax;
                    GUILayout.Space(15);
                }
                else
                {
                    GUILayout.FlexibleSpace();
                }

                if (max != null)
                {
                    max = EditorGUILayout.FloatField((float)max, GUILayout.MaxWidth(30));
                    if (GUI.Button(new Rect(minMaxRect.width - 75, minMaxRect.y + 2, 15, 15), "-", EditorStyles.miniButton))
                        max = null;
                }
                else if (GUI.Button(new Rect(minMaxRect.width - 45, minMaxRect.y + 2, 15, 15), "+", EditorStyles.miniButton))
                    max = 999;

                generalStyle.alignment = TextAnchor.MiddleLeft;

                EditorGUILayout.LabelField("MAX", generalStyle, GUILayout.MaxWidth(30));

                EditorGUILayout.EndHorizontal();

                value = DrawAttributeValue(min, max, attribute);

            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(attribute, attribute.name + "[" + attribute.Name + " - " + attribute.Alias + "]");
                attribute.Name = attributeName;
                attribute.Alias = attributeAlias;
                attribute.SetValue(value);

                if (min != null)
                {
                    attribute.SetMin((float)min);
                }
                else
                {
                    attribute.LimitMinimum = false;
                }

                if (max != null)
                {
                    attribute.SetMax((float)max);
                }
                else
                {
                    attribute.LimitMaximum = false;
                }
            }
        }
    }

    private float DrawAttributeValue(float? min, float? max, EntityAttribute attribute)
    {
        float value = attribute.Value;
        if (min != null && max != null)
        {
            value = EditorGUILayout.Slider(GUIContent.none, attribute.Value, (float)min, (float)max);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            value = EditorGUILayout.FloatField(attribute.Value, GUILayout.MaxWidth(50));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        return value;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Rect r = EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        displayAttributes = EditorGUILayout.Foldout(displayAttributes, "| ATTRIBUTES |", foldoutStyle);

        if (displayAttributes)
        {

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, scrollViewStyle, GUILayout.MinHeight(30), GUILayout.MaxHeight(240), GUILayout.Height(attributes.Length*40));

            DrawAttributes();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ADD ATTRIBUTE", EditorStyles.miniButton))
            {
                AddAttribute();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private void ToggleAttributeDisplay(int attributeIndex)
    {
        attributesVisibility[attributeIndex] = !attributesVisibility[attributeIndex];
    }

    private void DrawButton(string label, int index, Action<int> callback, GUIStyle style = null)
    {
        if (style == null)
        {
            style = EditorStyles.miniButton;
        }
        if (GUILayout.Button(label, style, GUILayout.MaxWidth(20)))
        {
            callback(index);
        }
    }

    void RemoveAttribute(int attributeIndex)
    {
        if (EditorUtility.DisplayDialog("Delete attribute", "This action cannot be undone.", "Proceed", "Cancel"))
        {
            attributes[attributeIndex].hideFlags = HideFlags.HideAndDontSave;
            DestroyImmediate(attributes[attributeIndex], true);
            GUIUtility.ExitGUI();
        }
    }

    private void AddAttribute()
    {
        entity.gameObject.AddComponent<EntityAttribute>();
        PopulateAttributesList();
    }
}