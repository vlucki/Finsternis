namespace Finsternis
{
    using UnityEngine;
    using UnityEditor;
    using UnityQuery;

    [CustomPropertyDrawer(typeof(Skill))]
    public class SkillDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var skill = property.objectReferenceValue as Skill;
            if (skill && !skill.name.IsNullOrEmpty())
                label.text = skill.Name;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}