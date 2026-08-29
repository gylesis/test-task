using Project.Core;
using UnityEngine;

namespace Project.Combat
{
    public readonly struct ProjectileHit
    {
        public ProjectileHit(Vector3 point, Vector3 normal, IDamageable target, bool hitTarget, float damage)
        {
            Point = point;
            Normal = normal;
            Target = target;
            HitTarget = hitTarget;
            Damage = damage;
        }

        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public IDamageable Target { get; }
        public bool HitTarget { get; }
        public float Damage { get; }
    }
}
