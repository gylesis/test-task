using UnityEngine;

namespace Project.Combat
{
    public readonly struct ProjectileRequest
    {
        public ProjectileRequest(
            GameObject prefab,
            Vector3 position,
            Vector3 direction,
            float speed,
            float damage,
            float lifetime,
            float radius,
            int hitMask,
            int obstacleMask)
        {
            Prefab = prefab;
            Position = position;
            Direction = direction;
            Speed = speed;
            Damage = damage;
            Lifetime = lifetime;
            Radius = radius;
            HitMask = hitMask;
            ObstacleMask = obstacleMask;
        }

        public GameObject Prefab { get; }
        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public float Speed { get; }
        public float Damage { get; }
        public float Lifetime { get; }
        public float Radius { get; }
        public int HitMask { get; }
        public int ObstacleMask { get; }
    }
}
