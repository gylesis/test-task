using System;
using System.Collections.Generic;
using Project.Core;
using UnityEngine;

namespace Project.Configs
{
    [Serializable]
    public sealed class EnemySpawnEntry
    {
        [SerializeField] private EnemyConfig _enemy;
        [SerializeField, Min(0f)] private float _weight = 1f;

        public EnemyConfig Enemy => _enemy;
        public float Weight => _weight;
    }

    [CreateAssetMenu(fileName = "SpawnConfig", menuName = "Game/Spawn Config")]
    public sealed class SpawnConfig : ScriptableObject
    {
        [Header("Roster")]
        [SerializeField] private List<EnemySpawnEntry> _enemies = new();

        [Header("Population")]
        [SerializeField, Min(1)] private int _minAliveEnemies = 5;
        [SerializeField, Min(1)] private int _maxAliveEnemies = 30;
        [SerializeField, Min(0f)] private float _spawnInterval = 1.5f;
        [SerializeField, Min(1)] private int _spawnBatchSize = 1;

        [Header("Placement")]
        [SerializeField] private MinMax _spawnRadius = new(12f, 18f);
        [SerializeField, Min(0f)] private float _arenaRadius = 40f;

        [Header("Difficulty")]
        [SerializeField, Min(0f)] private float _extraEnemyPerSecond = 0.05f;

        public IReadOnlyList<EnemySpawnEntry> Enemies => _enemies;
        public int MinAliveEnemies => _minAliveEnemies;
        public int MaxAliveEnemies => _maxAliveEnemies;
        public float SpawnInterval => _spawnInterval;
        public int SpawnBatchSize => _spawnBatchSize;
        public MinMax SpawnRadius => _spawnRadius;
        public float ArenaRadius => _arenaRadius;
        public float ExtraEnemyPerSecond => _extraEnemyPerSecond;

        public EnemyConfig PickRandom()
        {
            var total = 0f;
            for (var i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i]?.Enemy != null)
                    total += Mathf.Max(0f, _enemies[i].Weight);
            }

            if (total <= 0f)
                return null;

            var roll = UnityEngine.Random.value * total;
            for (var i = 0; i < _enemies.Count; i++)
            {
                var entry = _enemies[i];
                if (entry?.Enemy == null)
                    continue;

                roll -= Mathf.Max(0f, entry.Weight);
                if (roll <= 0f)
                    return entry.Enemy;
            }

            return null;
        }
    }
}
