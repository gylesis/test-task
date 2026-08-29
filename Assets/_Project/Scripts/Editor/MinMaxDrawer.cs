using Project.Core;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    [CustomPropertyDrawer(typeof(MinMax))]
    public sealed class MinMaxDrawer : PropertyDrawer
    {
        private const float LabelWidth = 30f;
        private const float Spacing = 6f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var min = property.FindPropertyRelative("_min");
            var max = property.FindPropertyRelative("_max");

            var fieldWidth = (contentRect.width - Spacing) * 0.5f;
            var minRect = new Rect(contentRect.x, contentRect.y, fieldWidth, contentRect.height);
            var maxRect = new Rect(contentRect.x + fieldWidth + Spacing, contentRect.y, fieldWidth, contentRect.height);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(minRect, min, new GUIContent("min"));
            EditorGUI.PropertyField(maxRect, max, new GUIContent("max"));

            if (EditorGUI.EndChangeCheck())
            {
                min.floatValue = Mathf.Max(0f, min.floatValue);
                max.floatValue = Mathf.Max(min.floatValue, max.floatValue);
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
