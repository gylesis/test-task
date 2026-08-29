using System.Collections.Generic;
using UnityEngine;

namespace Project.Core
{
    public sealed class EnemyRegistry
    {
        private readonly List<IDamageable> _enemies = new();

        public IReadOnlyList<IDamageable> Enemies => _enemies;
        public int Count => _enemies.Count;

        public void Register(IDamageable enemy)
        {
            if (enemy != null && !_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        public void Unregister(IDamageable enemy)
        {
            _enemies.Remove(enemy);
        }

        public IDamageable FindNearest(Vector3 origin, float maxDistance)
        {
            IDamageable best = null;
            var bestSqr = maxDistance * maxDistance;

            for (var i = 0; i < _enemies.Count; i++)
            {
                var candidate = _enemies[i];
                if (candidate == null || !candidate.IsAlive || candidate.Transform == null)
                    continue;

                var sqr = (candidate.Transform.position - origin).sqrMagnitude;
                if (sqr > bestSqr)
                    continue;

                bestSqr = sqr;
                best = candidate;
            }

            return best;
        }

        public void Clear()
        {
            _enemies.Clear();
        }
    }
}
