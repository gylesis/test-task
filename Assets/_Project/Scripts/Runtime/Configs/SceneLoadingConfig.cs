using UnityEngine;

namespace Project.Configs
{
    [CreateAssetMenu(fileName = "SceneLoadingConfig", menuName = "Game/Scene Loading Config")]
    public sealed class SceneLoadingConfig : ScriptableObject
    {
        [Header("Scenes")]
        [SerializeField] private string _gameSceneName = "Game";

        [Header("Curtain")]
        [SerializeField, Min(0f)] private float _fadeInDuration = 0.25f;
        [SerializeField, Min(0f)] private float _fadeOutDuration = 0.35f;
        [SerializeField, Min(0f)] private float _minimumDisplayTime = 0.6f;

        [Header("Warmup")]
        [SerializeField, Min(0)] private int _warmupProjectiles = 32;
        [SerializeField, Min(0)] private int _warmupEnemies = 6;
        [SerializeField, Min(0)] private int _warmupPopups = 16;
        [SerializeField, Min(1)] private int _warmupItemsPerFrame = 6;

        public string GameSceneName => _gameSceneName;
        public float FadeInDuration => _fadeInDuration;
        public float FadeOutDuration => _fadeOutDuration;
        public float MinimumDisplayTime => _minimumDisplayTime;
        public int WarmupProjectiles => _warmupProjectiles;
        public int WarmupEnemies => _warmupEnemies;
        public int WarmupPopups => _warmupPopups;
        public int WarmupItemsPerFrame => _warmupItemsPerFrame;
    }
}
