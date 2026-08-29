using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace Project.Combat
{
    public sealed class ProjectileService : IDisposable
    {
        private const int DefaultCapacity = 32;

        private readonly IObjectResolver _resolver;
        private readonly Dictionary<GameObject, ObjectPool<Projectile>> _pools = new();
        private readonly Transform _root;
        private Action<ProjectileHit> _onHit;

        public ProjectileService(IObjectResolver resolver)
        {
            _resolver = resolver;
            _root = new GameObject("[Projectiles]").transform;
        }

        public void SetHitCallback(Action<ProjectileHit> onHit)
        {
            _onHit = onHit;
        }

        public void Spawn(in ProjectileRequest request)
        {
            var pool = GetPool(request.Prefab);
            if (pool == null)
                return;

            pool.Get().Launch(
                request.Position,
                request.Direction,
                request.Speed,
                request.Damage,
                request.Lifetime,
                request.Radius,
                request.HitMask,
                request.ObstacleMask);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            var pool = GetPool(prefab);
            if (pool == null || count <= 0)
                return;

            var buffer = new Projectile[count];

            for (var i = 0; i < count; i++)
                buffer[i] = pool.Get();

            for (var i = 0; i < count; i++)
                pool.Release(buffer[i]);
        }

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
                pool?.Dispose();

            _pools.Clear();

            if (_root != null)
                UnityEngine.Object.Destroy(_root.gameObject);
        }

        private ObjectPool<Projectile> GetPool(GameObject prefab)
        {
            if (prefab == null)
                return null;

            if (_pools.TryGetValue(prefab, out var existing))
                return existing;

            var pool = new ObjectPool<Projectile>(
                () => Create(prefab),
                projectile => SetActiveSafe(projectile, true),
                projectile => SetActiveSafe(projectile, false),
                DestroySafe,
                false,
                DefaultCapacity,
                DefaultCapacity * 8);

            _pools[prefab] = pool;
            return pool;
        }

        private static void SetActiveSafe(Projectile projectile, bool state)
        {
            if (projectile != null)
                projectile.gameObject.SetActive(state);
        }

        private static void DestroySafe(Projectile projectile)
        {
            if (projectile != null)
                UnityEngine.Object.Destroy(projectile.gameObject);
        }

        private Projectile Create(GameObject prefab)
        {
            var instance = _resolver.Instantiate(prefab, _root);
            var projectile = instance.GetComponent<Projectile>();

            if (projectile == null)
                projectile = instance.AddComponent<Projectile>();

            projectile.Bind(p => Release(prefab, p), hit => _onHit?.Invoke(hit));
            return projectile;
        }

        private void Release(GameObject prefab, Projectile projectile)
        {
            if (projectile == null || !projectile.gameObject.activeSelf)
                return;

            if (_pools.TryGetValue(prefab, out var pool))
                pool.Release(projectile);
        }
    }
}
