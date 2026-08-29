using Project.Configs;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    [CustomEditor(typeof(EnemyConfig))]
    public sealed class EnemyConfigEditor : UnityEditor.Editor
    {
        private const string RangedPrefsKey = "Project.EnemyConfig.Ranged";
        private const string AdvancedPrefsKey = "Project.EnemyConfig.Advanced";
        private const string DeathPrefsKey = "Project.EnemyConfig.Death";

        private static readonly string[] MainFields =
        {
            "_prefab",
            "_maxHealth",
            "_moveSpeed",
            "_damage",
            "_attackType"
        };

        private static readonly string[] DeathFields =
        {
            "_useRagdoll",
            "_deathImpulse",
            "_deathImpulseUp",
            "_deathDelay"
        };

        private static readonly string[] RangedFields =
        {
            "_keepDistance",
            "_projectilePrefab",
            "_projectileSpeed",
            "_projectileLifetime",
            "_projectileRadius",
            "_muzzleHeight",
            "_retreatSpeedMultiplier",
            "_aimDelay",
            "_hitMask",
            "_obstacleMask"
        };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Основное", EditorStyles.boldLabel);

            foreach (var field in MainFields)
                DrawField(field);

            EditorGUILayout.Space(8f);

            var isRanged = serializedObject.FindProperty("_attackType").enumValueIndex ==
                           (int)EnemyAttackType.Ranged;

            if (isRanged)
                DrawFoldout(RangedPrefsKey, "Дальний бой", RangedFields, true);

            DrawFoldout(DeathPrefsKey, "Смерть", DeathFields, false);

            DrawAdvanced();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAdvanced()
        {
            var expanded = EditorPrefs.GetBool(AdvancedPrefsKey, false);
            var newExpanded = EditorGUILayout.Foldout(expanded, "Тонкая настройка", true, EditorStyles.foldoutHeader);

            if (newExpanded != expanded)
                EditorPrefs.SetBool(AdvancedPrefsKey, newExpanded);

            if (!newExpanded)
                return;

            EditorGUI.indentLevel++;

            var property = serializedObject.GetIterator();
            var enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.name == "m_Script")
                    continue;

                if (System.Array.IndexOf(MainFields, property.name) >= 0)
                    continue;

                if (System.Array.IndexOf(DeathFields, property.name) >= 0)
                    continue;

                if (System.Array.IndexOf(RangedFields, property.name) >= 0)
                    continue;

                EditorGUILayout.PropertyField(property, true);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawFoldout(string prefsKey, string title, string[] fields, bool defaultState)
        {
            var expanded = EditorPrefs.GetBool(prefsKey, defaultState);
            var newExpanded = EditorGUILayout.Foldout(expanded, title, true, EditorStyles.foldoutHeader);

            if (newExpanded != expanded)
                EditorPrefs.SetBool(prefsKey, newExpanded);

            if (!newExpanded)
                return;

            EditorGUI.indentLevel++;

            foreach (var field in fields)
                DrawField(field);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(8f);
        }

        private void DrawField(string name)
        {
            var property = serializedObject.FindProperty(name);

            if (property != null)
                EditorGUILayout.PropertyField(property, true);
        }
    }
}
