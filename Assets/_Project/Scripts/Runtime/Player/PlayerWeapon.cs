using System;
using Project.Combat;
using Project.Configs;
using Project.Core;
using Project.Vfx;
using UnityEngine;

namespace Project.Player
{
    public sealed class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private Transform _muzzle;

        private WeaponConfig _config;
        private EnemyRegistry _enemies;
        private ProjectileService _projectiles;
        private VfxController _vfx;
        private float _cooldown;

        public IDamageable CurrentTarget { get; private set; }

        public event Action Fired;

        public void Initialize(WeaponConfig config, EnemyRegistry enemies, ProjectileService projectiles, VfxController vfx)
        {
            _config = config;
            _enemies = enemies;
            _projectiles = projectiles;
            _vfx = vfx;
        }

        public Vector3 MuzzlePosition => _muzzle != null
            ? _muzzle.position
            : transform.position + Vector3.up * (_config != null ? _config.MuzzleHeight : 1f);

        public void Tick(float deltaTime)
        {
            if (_config == null)
                return;

            _cooldown -= deltaTime;
            CurrentTarget = _enemies.FindNearest(transform.position, _config.Range);

            if (CurrentTarget == null || _cooldown > 0f)
                return;

            if (!IsAimedAt(CurrentTarget))
                return;

            Fire(CurrentTarget);
            Fired?.Invoke();
            _cooldown = _config.FireInterval;
        }

        public Vector3 GetAimDirection()
        {
            if (CurrentTarget == null || CurrentTarget.Transform == null)
                return transform.forward;

            var direction = CurrentTarget.Transform.position - transform.position;
            direction.y = 0f;
            return direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
        }

        private bool IsAimedAt(IDamageable target)
        {
            var toTarget = GetAimDirection();
            var forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
                return false;

            return Vector3.Angle(forward, toTarget) <= _config.MaxAimAngle;
        }

        private void Fire(IDamageable target)
        {
            var muzzle = MuzzlePosition;
            var direction = target.Transform.position - muzzle;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            direction.Normalize();

            _projectiles.Spawn(new ProjectileRequest(
                _config.ProjectilePrefab,
                muzzle,
                direction,
                _config.ProjectileSpeed,
                _config.Damage.Roll(),
                _config.ProjectileLifetime,
                _config.ProjectileRadius,
                _config.HitMask,
                _config.ObstacleMask));
            _vfx?.PlayDirected(VfxIds.MuzzleFlash, muzzle, direction);
        }
    }
}
