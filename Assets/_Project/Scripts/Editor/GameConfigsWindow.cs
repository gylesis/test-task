using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.Editor
{
    public sealed class GameConfigsWindow : EditorWindow
    {
        private const float SidebarWidth = 240f;

        private static readonly string[] CoreOrder =
        {
            "GameConfig",
            "PlayerConfig",
            "WeaponConfig",
            "SpawnConfig",
            "CameraConfig",
            "InputConfig",
            "VfxConfig",
            "DamagePopupConfig",
            "SceneLoadingConfig"
        };

        private readonly List<ScriptableObject> _core = new();
        private readonly List<ScriptableObject> _enemies = new();

        private ScriptableObject _selected;
        private UnityEditor.Editor _inspector;
        private Vector2 _sidebarScroll;
        private Vector2 _inspectorScroll;
        private string _search = string.Empty;

        [MenuItem("Tools/Game/Configs %#c", priority = 1)]
        public static void Open()
        {
            var window = GetWindow<GameConfigsWindow>("Game Configs");
            window.minSize = new Vector2(680f, 420f);
            window.Reload();
        }

        private void OnEnable()
        {
            Reload();
        }

        private void OnDisable()
        {
            DestroyInspector();
        }

        private void OnFocus()
        {
            Reload();
        }

        private void OnGUI()
        {
            DrawToolbar();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSidebar();
                DrawInspector();
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                _search = GUILayout.TextField(_search, EditorStyles.toolbarSearchField, GUILayout.Width(200f));

                if (GUILayout.Button("+ Новый враг", EditorStyles.toolbarButton, GUILayout.Width(110f)))
                    CreateEnemyWindow.Open();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Обновить", EditorStyles.toolbarButton, GUILayout.Width(80f)))
                    Reload();

                using (new EditorGUI.DisabledScope(_selected == null))
                {
                    if (GUILayout.Button("Показать в проекте", EditorStyles.toolbarButton, GUILayout.Width(140f)))
                        EditorGUIUtility.PingObject(_selected);
                }

                if (GUILayout.Button("Сохранить", EditorStyles.toolbarButton, GUILayout.Width(80f)))
                    AssetDatabase.SaveAssets();
            }
        }

        private void DrawSidebar()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(SidebarWidth)))
            using (var scroll = new EditorGUILayout.ScrollViewScope(_sidebarScroll))
            {
                _sidebarScroll = scroll.scrollPosition;

                DrawGroup("Основные", _core);
                DrawGroup("Враги", _enemies);

                if (_core.Count == 0 && _enemies.Count == 0)
                    EditorGUILayout.HelpBox("Конфиги не найдены.\nЗапусти Tools → Game → Setup Scene.", MessageType.Info);
            }
        }

        private void DrawGroup(string title, List<ScriptableObject> items)
        {
            var filtered = items
                .Where(item => item != null && Matches(item.name))
                .ToList();

            if (filtered.Count == 0)
                return;

            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            foreach (var item in filtered)
            {
                var selected = item == _selected;
                var style = selected ? EditorStyles.miniButtonMid : EditorStyles.label;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(8f);

                    if (GUILayout.Button(item.name, style, GUILayout.ExpandWidth(true)))
                        Select(item);
                }
            }

            EditorGUILayout.Space(6f);
        }

        private void DrawInspector()
        {
            using (new EditorGUILayout.VerticalScope())
            using (var scroll = new EditorGUILayout.ScrollViewScope(_inspectorScroll))
            {
                _inspectorScroll = scroll.scrollPosition;

                if (_selected == null)
                {
                    EditorGUILayout.HelpBox("Выбери конфиг слева.", MessageType.None);
                    return;
                }

                EditorGUILayout.LabelField(_selected.name, EditorStyles.largeLabel);
                EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(_selected), EditorStyles.miniLabel);
                EditorGUILayout.Space(4f);

                if (_inspector == null)
                    _inspector = UnityEditor.Editor.CreateEditor(_selected);

                _inspector.OnInspectorGUI();
            }
        }

        private bool Matches(string name)
        {
            return string.IsNullOrEmpty(_search)
                   || name.IndexOf(_search, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void Select(ScriptableObject config)
        {
            if (_selected == config)
                return;

            _selected = config;
            DestroyInspector();
            GUI.FocusControl(null);
        }

        private void DestroyInspector()
        {
            if (_inspector == null)
                return;

            DestroyImmediate(_inspector);
            _inspector = null;
        }

        private void Reload()
        {
            _core.Clear();
            _enemies.Clear();

            var all = AssetDatabase.FindAssets("t:ScriptableObject", new[] { ProjectPaths.Configs })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .Where(asset => asset != null)
                .ToList();

            foreach (var asset in all)
            {
                var path = AssetDatabase.GetAssetPath(asset);

                if (path.StartsWith(ProjectPaths.EnemyConfigs))
                    _enemies.Add(asset);
                else
                    _core.Add(asset);
            }

            _core.Sort(CompareCore);
            _enemies.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            if (_selected != null && !_core.Contains(_selected) && !_enemies.Contains(_selected))
                Select(null);

            Repaint();
        }

        private static int CompareCore(ScriptableObject a, ScriptableObject b)
        {
            var indexA = System.Array.IndexOf(CoreOrder, a.name);
            var indexB = System.Array.IndexOf(CoreOrder, b.name);

            if (indexA < 0) indexA = int.MaxValue;
            if (indexB < 0) indexB = int.MaxValue;

            return indexA != indexB
                ? indexA.CompareTo(indexB)
                : string.CompareOrdinal(a.name, b.name);
        }
    }
}
