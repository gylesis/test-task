using Project.Configs;
using Project.Core;
using Project.Player;
using UnityEngine;
using VContainer.Unity;

namespace Project.Enemies
{
    public sealed class EnemySpawner : IStartable, ITickable
    {
        private readonly SpawnConfig _config;
        private readonly EnemyFactory _factory;
        private readonly EnemyRegistry _registry;
        private readonly PlayerController _player;
        private readonly GameStateService _gameState;

        private float _spawnCooldown;

        public EnemySpawner(GameConfig gameConfig, EnemyFactory factory, EnemyRegistry registry, PlayerController player, GameStateService gameState)
        {
            _config = gameConfig.Spawn;
            _factory = factory;
            _registry = registry;
            _player = player;
            _gameState = gameState;
        }

        public void Start()
        {
            for (var i = 0; i < _config.MinAliveEnemies; i++)
            {
                if (!SpawnOne())
                    return;
            }
        }

        public void Tick()
        {
            if (!_gameState.IsRunning)
                return;

            var deltaTime = Time.deltaTime;

            var desired = DesiredCount();

            var aliveEnemiesCount = _registry.Count;
            
            while (aliveEnemiesCount < _config.MinAliveEnemies && aliveEnemiesCount < _config.MaxAliveEnemies)
            {
                if (!SpawnOne())
                    return;

                aliveEnemiesCount++;
            }

            _spawnCooldown -= deltaTime;
            if (_spawnCooldown > 0f)
                return;

            _spawnCooldown = _config.SpawnInterval;

            if (aliveEnemiesCount >= desired)
                return;

            for (var i = 0; i < _config.SpawnBatchSize && aliveEnemiesCount < _config.MaxAliveEnemies; i++)
            {
                if (!SpawnOne())
                    return;

                aliveEnemiesCount++;
            }
        }

        private int DesiredCount()
        {
            var growth = Mathf.FloorToInt(_gameState.Elapsed.Value * _config.ExtraEnemyPerSecond);
            return Mathf.Clamp(_config.MinAliveEnemies + growth, _config.MinAliveEnemies, _config.MaxAliveEnemies);
        }

        private bool SpawnOne()
        {
            var enemyConfig = _config.PickRandom();
            if (enemyConfig == null)
                return false;

            return _factory.Spawn(enemyConfig, GetSpawnPosition()) != null;
        }

        private Vector3 GetSpawnPosition()
        {
            var center = _player.transform.position;
            var angle = Random.value * Mathf.PI * 2f;
            var radius = _config.SpawnRadius.Roll();

            var position = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

            if (_config.ArenaRadius > 0f)
            {
                var flat = new Vector3(position.x, 0f, position.z);
                if (flat.magnitude > _config.ArenaRadius)
                    flat = flat.normalized * _config.ArenaRadius;

                position = new Vector3(flat.x, position.y, flat.z);
            }

            position.y = 0.1f;
            return position;
        }
    }
}
