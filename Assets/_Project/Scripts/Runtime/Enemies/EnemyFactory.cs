using System;
using System.Collections.Generic;
using Project.Configs;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Project.Enemies
{
    public sealed class EnemyFactory : IDisposable
    {
        private const int DefaultCapacity = 16;

        private readonly IObjectResolver _resolver;
        private readonly Transform _root;
        private readonly Dictionary<GameObject, ObjectPool<EnemyController>> _pools = new();
        private readonly Dictionary<EnemyController, GameObject> _origins = new();
        private readonly List<EnemyController> _alive = new();

        public EnemyFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
            _root = new GameObject("[Enemies]").transform;
        }

        public EnemyController Spawn(EnemyConfig config, Vector3 position)
        {
            var pool = GetPool(config?.Prefab);

            if (pool == null)
                return null;

            var enemy = pool.Get();
            enemy.transform.SetPositionAndRotation(position, Quaternion.identity);
            enemy.Initialize(config, Despawn);

            _alive.Add(enemy);
            return enemy;
        }

        public void Prewarm(EnemyConfig config, int count)
        {
            var pool = GetPool(config?.Prefab);

            if (pool == null || count <= 0)
                return;

            var buffer = new EnemyController[count];

            for (var i = 0; i < count; i++)
                buffer[i] = pool.Get();

            for (var i = 0; i < count; i++)
                pool.Release(buffer[i]);
        }

        public void DespawnAll()
        {
            for (var i = _alive.Count - 1; i >= 0; i--)
                Despawn(_alive[i]);
        }

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
                pool?.Dispose();

            _pools.Clear();
            _origins.Clear();
            _alive.Clear();

            if (_root != null)
                UnityEngine.Object.Destroy(_root.gameObject);
        }

        private ObjectPool<EnemyController> GetPool(GameObject prefab)
        {
            if (prefab == null)
                return null;

            if (_pools.TryGetValue(prefab, out var existing))
                return existing;

            var pool = new ObjectPool<EnemyController>(
                () => Create(prefab),
                enemy => SetActiveSafe(enemy, true),
                enemy => SetActiveSafe(enemy, false),
                DestroySafe,
                false,
                DefaultCapacity,
                DefaultCapacity * 8);

            _pools[prefab] = pool;
            return pool;
        }

        private EnemyController Create(GameObject prefab)
        {
            var instance = _resolver.Instantiate(prefab, _root);
            var enemy = instance.GetComponent<EnemyController>();

            if (enemy != null)
                _origins[enemy] = prefab;

            return enemy;
        }

        private void Despawn(EnemyController enemy)
        {
            if (enemy == null)
                return;

            _alive.Remove(enemy);
            enemy.ResetForPool();

            if (!enemy.gameObject.activeSelf)
                return;

            if (_origins.TryGetValue(enemy, out var prefab) && _pools.TryGetValue(prefab, out var pool))
                pool.Release(enemy);
        }

        private static void SetActiveSafe(EnemyController enemy, bool state)
        {
            if (enemy != null)
                enemy.gameObject.SetActive(state);
        }

        private static void DestroySafe(EnemyController enemy)
        {
            if (enemy != null)
                UnityEngine.Object.Destroy(enemy.gameObject);
        }
    }
}
