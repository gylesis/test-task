using Project.Core;
using UnityEngine;

namespace Project.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Game/Weapon Config")]
    public sealed class WeaponConfig : ScriptableObject
    {
        [Header("Damage")]
        [SerializeField] private MinMax _damage = new(9f, 14f);
        [SerializeField, Min(0.01f)] private float _fireRate = 3f;
        [SerializeField, Min(0.1f)] private float _range = 12f;

        [Header("Projectile")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField, Min(0.1f)] private float _projectileSpeed = 25f;
        [SerializeField, Min(0.1f)] private float _projectileLifetime = 2f;
        [SerializeField, Min(0.01f)] private float _projectileRadius = 0.25f;

        [Header("Collision")]
        [SerializeField] private LayerMask _hitMask;
        [SerializeField] private LayerMask _obstacleMask;

        [Header("Aiming")]
        [SerializeField, Range(1f, 180f)] private float _maxAimAngle = 25f;
        [SerializeField] private float _muzzleHeight = 1f;

        public MinMax Damage => _damage;
        public float FireInterval => 1f / Mathf.Max(0.01f, _fireRate);
        public float Range => _range;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public float ProjectileRadius => _projectileRadius;
        public int HitMask => _hitMask.value;
        public int ObstacleMask => _obstacleMask.value;
        public float MaxAimAngle => _maxAimAngle;
        public float MuzzleHeight => _muzzleHeight;
    }
}
