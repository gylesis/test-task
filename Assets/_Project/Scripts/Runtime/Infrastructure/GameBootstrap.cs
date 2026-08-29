using System.Collections;
using Project.Combat;
using Project.Configs;
using Project.Core;
using Project.Enemies;
using Project.Vfx;
using VContainer.Unity;

namespace Project.Infrastructure
{
    public sealed class GameBootstrap : IStartable
    {
        private readonly GameConfig _gameConfig;
        private readonly SceneLoadingConfig _loadingConfig;
        private readonly ProjectileService _projectiles;
        private readonly EnemyFactory _enemyFactory;
        private readonly VfxController _vfx;
        private readonly Combat.DamagePopupService _popups;
        private readonly GameStateService _gameState;
        private readonly SceneLoader _sceneLoader;
        private readonly CoroutineRunner _runner;

        public GameBootstrap(
            GameConfig gameConfig,
            SceneLoadingConfig loadingConfig,
            ProjectileService projectiles,
            EnemyFactory enemyFactory,
            VfxController vfx,
            Combat.DamagePopupService popups,
            GameStateService gameState,
            SceneLoader sceneLoader,
            CoroutineRunner runner)
        {
            _gameConfig = gameConfig;
            _loadingConfig = loadingConfig;
            _projectiles = projectiles;
            _enemyFactory = enemyFactory;
            _vfx = vfx;
            _popups = popups;
            _gameState = gameState;
            _sceneLoader = sceneLoader;
            _runner = runner;
        }

        public void Start()
        {
            _projectiles.SetHitCallback(OnProjectileHit);
            _runner.Run(WarmupRoutine());
        }

        private void OnProjectileHit(Combat.ProjectileHit hit)
        {
            var id = hit.HitTarget ? VfxIds.EnemyHit : VfxIds.Impact;
            _vfx.PlayDirected(id, hit.Point, hit.Normal);

            if (hit.HitTarget)
                _popups.Show(hit.Damage, hit.Point);
        }

        private System.Collections.Generic.IEnumerable<UnityEngine.GameObject> CollectProjectilePrefabs()
        {
            if (_gameConfig.Player.Weapon.ProjectilePrefab != null)
                yield return _gameConfig.Player.Weapon.ProjectilePrefab;

            foreach (var entry in _gameConfig.Spawn.Enemies)
            {
                var enemy = entry?.Enemy;
                if (enemy != null && enemy.IsRanged && enemy.ProjectilePrefab != null)
                    yield return enemy.ProjectilePrefab;
            }
        }

        private IEnumerator WarmupRoutine()
        {
            var batch = _loadingConfig.WarmupItemsPerFrame;

            _sceneLoader.ReportProgress("Подготовка снарядов", 0.75f);

            foreach (var prefab in CollectProjectilePrefabs())
            {
                for (var spawned = 0; spawned < _loadingConfig.WarmupProjectiles; spawned += batch)
                {
                    _projectiles.Prewarm(prefab, UnityEngine.Mathf.Min(batch, _loadingConfig.WarmupProjectiles - spawned));
                    yield return null;
                }
            }

            _popups.Prewarm(_loadingConfig.WarmupPopups);
            yield return null;

            _sceneLoader.ReportProgress("Подготовка врагов", 0.9f);

            foreach (var entry in _gameConfig.Spawn.Enemies)
            {
                if (entry?.Enemy == null)
                    continue;

                _enemyFactory.Prewarm(entry.Enemy, _loadingConfig.WarmupEnemies);
                yield return null;
            }

            _gameState.SetRunning();
            _sceneLoader.NotifySceneReady();
        }
    }
}
