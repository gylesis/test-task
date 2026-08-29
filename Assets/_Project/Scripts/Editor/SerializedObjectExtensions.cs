using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public static class SerializedObjectExtensions
    {
        public static SerializedObject Edit(this Object target)
        {
            return new SerializedObject(target);
        }

        public static SerializedObject Float(this SerializedObject so, string path, float value)
        {
            Find(so, path)?.SetFloat(value);
            return so;
        }

        public static SerializedObject Int(this SerializedObject so, string path, int value)
        {
            var property = Find(so, path);

            if (property != null)
                property.intValue = value;

            return so;
        }

        public static SerializedObject Enum(this SerializedObject so, string path, int index)
        {
            var property = Find(so, path);

            if (property != null)
                property.enumValueIndex = index;

            return so;
        }

        public static SerializedObject Mask(this SerializedObject so, string path, int mask)
        {
            var property = Find(so, path);

            if (property != null)
                property.intValue = mask;

            return so;
        }

        public static SerializedObject Vec3(this SerializedObject so, string path, Vector3 value)
        {
            var property = Find(so, path);

            if (property != null)
                property.vector3Value = value;

            return so;
        }

        public static SerializedObject Ref(this SerializedObject so, string path, Object value)
        {
            var property = Find(so, path);

            if (property != null)
                property.objectReferenceValue = value;

            return so;
        }

        public static SerializedObject Range(this SerializedObject so, string path, float min, float max)
        {
            var property = Find(so, path);

            if (property == null)
                return so;

            property.FindPropertyRelative("_min").floatValue = min;
            property.FindPropertyRelative("_max").floatValue = max;
            return so;
        }

        public static void Save(this SerializedObject so)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(this SerializedProperty property, float value)
        {
            property.floatValue = value;
        }

        private static SerializedProperty Find(SerializedObject so, string path)
        {
            var property = so.FindProperty(path);

            if (property == null)
                Debug.LogError($"Field '{path}' not found on {so.targetObject.GetType().Name}.");

            return property;
        }
    }
}
