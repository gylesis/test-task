using System.IO;
using Project.Configs;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public sealed class CreateEnemyWindow : EditorWindow
    {
        private string _name = "NewEnemy";
        private GameObject _model;
        private float _health = 30f;
        private float _moveSpeed = 3f;
        private float _damageMin = 8f;
        private float _damageMax = 12f;
        private EnemyAttackType _attackType = EnemyAttackType.Melee;
        private float _spawnWeight = 1f;
        private bool _addToSpawn = true;

        public static void Open()
        {
            var window = GetWindow<CreateEnemyWindow>(true, "Новый враг");
            window.minSize = new Vector2(380f, 300f);
            window.maxSize = new Vector2(520f, 340f);
            window.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "Создаст EnemyConfig с указанными параметрами и добавит его в SpawnConfig.",
                MessageType.None);

            EditorGUILayout.Space(4f);

            _name = EditorGUILayout.TextField("Имя", _name);
            _model = (GameObject)EditorGUILayout.ObjectField("Модель", _model, typeof(GameObject), false);

            EditorGUILayout.Space(4f);

            _health = EditorGUILayout.FloatField("Здоровье", _health);
            _moveSpeed = EditorGUILayout.FloatField("Скорость", _moveSpeed);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Урон");
                _damageMin = EditorGUILayout.FloatField(_damageMin);
                _damageMax = EditorGUILayout.FloatField(_damageMax);
            }

            _attackType = (EnemyAttackType)EditorGUILayout.EnumPopup("Тип атаки", _attackType);

            EditorGUILayout.Space(4f);

            _addToSpawn = EditorGUILayout.Toggle("Добавить в спавнер", _addToSpawn);

            using (new EditorGUI.DisabledScope(!_addToSpawn))
                _spawnWeight = EditorGUILayout.FloatField("Вес спавна", _spawnWeight);

            EditorGUILayout.Space(10f);

            var invalid = string.IsNullOrWhiteSpace(_name);

            if (invalid)
                EditorGUILayout.HelpBox("Укажи имя врага.", MessageType.Warning);

            using (new EditorGUI.DisabledScope(invalid))
            {
                if (GUILayout.Button("Создать", GUILayout.Height(32f)))
                    Create();
            }
        }

        private void Create()
        {
            EnsureConfigsFolder();

            var assetName = _name.StartsWith("Enemy_") ? _name : $"Enemy_{_name}";
            var path = AssetDatabase.GenerateUniqueAssetPath($"{ProjectPaths.EnemyConfigs}/{assetName}.asset");

            var config = ScriptableObject.CreateInstance<EnemyConfig>();
            AssetDatabase.CreateAsset(config, path);

            var damageMax = Mathf.Max(_damageMin, _damageMax);

            config.Edit()
                .Ref("_modelPrefab", _model)
                .Float("_maxHealth", Mathf.Max(1f, _health))
                .Float("_moveSpeed", Mathf.Max(0f, _moveSpeed))
                .Range("_damage", Mathf.Max(0f, _damageMin), damageMax)
                .Enum("_attackType", (int)_attackType)
                .Vec3("_modelScale", Vector3.one)
                .Float("_rotationSpeed", 540f)
                .Float("_attackInterval", _attackType == EnemyAttackType.Ranged ? 1.4f : 1f)
                .Float("_attackRange", _attackType == EnemyAttackType.Ranged ? 11f : 1.4f)
                .Float("_attackRangeExitMultiplier", _attackType == EnemyAttackType.Ranged ? 1.25f : 1.15f)
                .Float("_radius", 0.5f)
                .Float("_height", 2f)
                .Float("_healthBarHeight", 2.4f)
                .Float("_spawnDelay", 0.2f)
                .Float("_deathDelay", 2.5f)
                .Float("_separationRadius", 1.2f)
                .Float("_separationWeight", 1.5f)
                .Float("_animationSpeedReference", Mathf.Max(0.01f, _moveSpeed))
                .Save();

            if (_attackType == EnemyAttackType.Ranged)
            {
                config.Edit()
                    .Float("_keepDistance", 5f)
                    .Float("_projectileSpeed", 13f)
                    .Float("_projectileLifetime", 3f)
                    .Float("_projectileRadius", 0.22f)
                    .Float("_muzzleHeight", 1.2f)
                    .Float("_retreatSpeedMultiplier", 0.6f)
                    .Float("_aimDelay", 0.25f)
                    .Mask("_hitMask", LayerMaskOf("Player"))
                    .Mask("_obstacleMask", LayerMaskOf("Obstacle"))
                    .Save();
            }

            if (_addToSpawn)
                AddToSpawnConfig(config);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);

            Debug.Log($"Создан враг '{_name}' → {path}", config);
            Close();
        }

        private static void EnsureConfigsFolder()
        {
            if (AssetDatabase.IsValidFolder(ProjectPaths.EnemyConfigs))
                return;

            if (!AssetDatabase.IsValidFolder(ProjectPaths.Configs))
                AssetDatabase.CreateFolder(ProjectPaths.Root, "Configs");

            AssetDatabase.CreateFolder(ProjectPaths.Configs, "Enemies");
        }

        private static int LayerMaskOf(string layerName)
        {
            var index = LayerMask.NameToLayer(layerName);
            return index < 0 ? 0 : 1 << index;
        }

        private void AddToSpawnConfig(EnemyConfig config)
        {
            var spawnPath = $"{ProjectPaths.Configs}/SpawnConfig.asset";
            var spawn = AssetDatabase.LoadAssetAtPath<SpawnConfig>(spawnPath);

            if (spawn == null)
            {
                Debug.LogWarning($"SpawnConfig не найден по пути {spawnPath}, враг создан без добавления в спавнер.");
                return;
            }

            var so = spawn.Edit();
            var list = so.FindProperty("_enemies");
            var index = list.arraySize;
            list.arraySize = index + 1;

            var element = list.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("_enemy").objectReferenceValue = config;
            element.FindPropertyRelative("_weight").floatValue = Mathf.Max(0f, _spawnWeight);
            so.Save();

            EditorUtility.SetDirty(spawn);
        }
    }
}
