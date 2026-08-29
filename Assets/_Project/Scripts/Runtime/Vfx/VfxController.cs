using System.Collections.Generic;
using Project.Configs;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Project.Vfx
{
    public sealed class VfxController : MonoBehaviour
    {
        private readonly Dictionary<string, ObjectPool<Effect>> _pools = new();

        private VfxConfig _config;

        [Inject]
        public void Construct(GameConfig gameConfig)
        {
            _config = gameConfig.Vfx;
        }

        public void Play(string id, Vector3 position, Quaternion rotation)
        {
            var pool = GetPool(id);
            if (pool == null)
                return;

            pool.Get().Play(position, rotation);
        }

        public void Play(string id, Vector3 position)
        {
            Play(id, position, Quaternion.identity);
        }

        public void PlayDirected(string id, Vector3 position, Vector3 direction)
        {
            var rotation = direction.sqrMagnitude > 0.0001f
                ? Quaternion.LookRotation(direction, Vector3.up)
                : Quaternion.identity;

            Play(id, position, rotation);
        }

        private ObjectPool<Effect> GetPool(string id)
        {
            if (_config == null || string.IsNullOrEmpty(id))
                return null;

            if (_pools.TryGetValue(id, out var existing))
                return existing;

            var entry = _config.Find(id);

            if (entry?.Prefab == null)
            {
                _pools[id] = null;
                return null;
            }

            var size = entry.PoolSize > 0 ? entry.PoolSize : _config.DefaultPoolSize;

            ObjectPool<Effect> pool = null;
            pool = new ObjectPool<Effect>(
                () => Create(entry.Prefab, pool),
                effect => SetActiveSafe(effect, true),
                effect => SetActiveSafe(effect, false),
                DestroySafe,
                false,
                size,
                size * 4);

            _pools[id] = pool;
            return pool;
        }

        private Effect Create(Effect prefab, ObjectPool<Effect> pool)
        {
            var instance = Instantiate(prefab, transform);
            instance.Finished += effect => Release(pool, effect);
            return instance;
        }

        private static void Release(ObjectPool<Effect> pool, Effect effect)
        {
            if (pool != null && effect != null && effect.gameObject.activeSelf)
                pool.Release(effect);
        }

        private static void SetActiveSafe(Effect effect, bool state)
        {
            if (effect != null)
                effect.gameObject.SetActive(state);
        }

        private static void DestroySafe(Effect effect)
        {
            if (effect != null)
                Destroy(effect.gameObject);
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools.Values)
                pool?.Dispose();

            _pools.Clear();
        }
    }
}
