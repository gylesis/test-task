using Project.Core;
using UnityEngine;

namespace Project.Configs
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Game/Enemy Config")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private GameObject _prefab;

        [Header("Stats")]
        [SerializeField, Min(1f)] private float _maxHealth = 30f;
        [SerializeField, Min(0f)] private float _moveSpeed = 3f;
        [SerializeField, Min(0f)] private float _rotationSpeed = 540f;
        [SerializeField, Min(0.01f)] private float _animationSpeedReference = 3.2f;

        [Header("Attack")]
        [SerializeField] private EnemyAttackType _attackType = EnemyAttackType.Melee;
        [SerializeField] private MinMax _damage = new(8f, 12f);
        [SerializeField, Min(0.05f)] private float _attackInterval = 1f;
        [SerializeField, Min(0.1f)] private float _attackRange = 1.6f;
        [SerializeField, Min(1f)] private float _attackRangeExitMultiplier = 1.15f;

        [Header("Ranged")]
        [SerializeField, Min(0f)] private float _keepDistance;
        [SerializeField, Range(0.1f, 1f)] private float _retreatSpeedMultiplier = 0.6f;
        [SerializeField, Min(0f)] private float _aimDelay = 0.25f;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField, Min(0.1f)] private float _projectileSpeed = 14f;
        [SerializeField, Min(0.1f)] private float _projectileLifetime = 3f;
        [SerializeField, Min(0.01f)] private float _projectileRadius = 0.2f;
        [SerializeField] private float _muzzleHeight = 1.2f;
        [SerializeField] private LayerMask _hitMask;
        [SerializeField] private LayerMask _obstacleMask;

        [Header("Body")]
        [SerializeField, Min(0.05f)] private float _radius = 0.5f;
        [SerializeField, Min(0.1f)] private float _height = 2f;
        [SerializeField] private float _healthBarHeight = 2f;
        [SerializeField] private bool _hideHealthBarWhenFull;

        [Header("Death")]
        [SerializeField] private bool _useRagdoll = true;
        [SerializeField] private MinMax _deathImpulse = new(45f, 80f);
        [SerializeField] private float _deathImpulseUp = 22f;
        [SerializeField, Min(0f)] private float _deathDelay = 2.5f;

        [Header("Behaviour")]
        [SerializeField, Min(0f)] private float _spawnDelay = 0.25f;
        [SerializeField, Min(0f)] private float _separationRadius = 1.2f;
        [SerializeField, Min(0f)] private float _separationWeight = 1.5f;

        public GameObject Prefab => _prefab;

        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float AnimationSpeedReference => _animationSpeedReference;

        public EnemyAttackType AttackType => _attackType;
        public bool IsRanged => _attackType == EnemyAttackType.Ranged;
        public MinMax Damage => _damage;
        public float AttackInterval => _attackInterval;
        public float AttackRange => _attackRange;
        public float AttackRangeExit => _attackRange * _attackRangeExitMultiplier;

        public float KeepDistance => _keepDistance;
        public float RetreatSpeed => _moveSpeed * _retreatSpeedMultiplier;
        public float AimDelay => _aimDelay;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public float ProjectileRadius => _projectileRadius;
        public float MuzzleHeight => _muzzleHeight;
        public int HitMask => _hitMask.value;
        public int ObstacleMask => _obstacleMask.value;

        public float Radius => _radius;
        public float Height => _height;
        public float HealthBarHeight => _healthBarHeight;
        public bool HideHealthBarWhenFull => _hideHealthBarWhenFull;

        public bool UseRagdoll => _useRagdoll;
        public MinMax DeathImpulse => _deathImpulse;
        public float DeathImpulseUp => _deathImpulseUp;
        public float DeathDelay => _deathDelay;

        public float SpawnDelay => _spawnDelay;
        public float SeparationRadius => _separationRadius;
        public float SeparationWeight => _separationWeight;
    }
}
