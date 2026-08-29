using System;
using Project.Core;
using UnityEngine;

namespace Project.Combat
{
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trail;

        private readonly Collider[] _overlap = new Collider[8];

        private Vector3 _direction;
        private float _speed;
        private float _damage;
        private float _radius;
        private float _lifeLeft;
        private int _hitMask;
        private int _obstacleMask;
        private bool _active;
        private Action<Projectile> _returnToPool;
        private Action<ProjectileHit> _onHit;

        public void Bind(Action<Projectile> returnToPool, Action<ProjectileHit> onHit)
        {
            _returnToPool = returnToPool;
            _onHit = onHit;
        }

        public void Launch(Vector3 position, Vector3 direction, float speed, float damage, float lifetime, float radius, int hitMask, int obstacleMask)
        {
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction, Vector3.up));

            if (_trail != null)
            {
                _trail.Clear();
                _trail.enabled = true;
                _trail.emitting = true;
            }

            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _lifeLeft = lifetime;
            _radius = radius;
            _hitMask = hitMask;
            _obstacleMask = obstacleMask;
            _active = true;
        }

        private void Update()
        {
            if (!_active)
                return;

            _lifeLeft -= Time.deltaTime;
            if (_lifeLeft <= 0f)
            {
                Release();
                return;
            }

            var step = _speed * Time.deltaTime;
            var origin = transform.position;
            var mask = _hitMask | _obstacleMask;

            if (TryHitOverlapping(origin, mask))
                return;

            if (Physics.SphereCast(origin, _radius, _direction, out var hit, step, mask, QueryTriggerInteraction.Collide))
            {
                Hit(hit.collider, hit.point);
                return;
            }

            transform.position = origin + _direction * step;
        }

        private bool TryHitOverlapping(Vector3 origin, int mask)
        {
            var count = Physics.OverlapSphereNonAlloc(origin, _radius, _overlap, _hitMask, QueryTriggerInteraction.Collide);

            for (var i = 0; i < count; i++)
            {
                var collider = _overlap[i];
                if (collider == null)
                    continue;

                var damageable = collider.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive)
                    continue;

                Hit(collider, origin);
                return true;
            }

            return false;
        }

        private void Hit(Collider collider, Vector3 point)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            var hitTarget = damageable != null && damageable.IsAlive;

            if (hitTarget)
                damageable.TakeDamage(_damage);

            _onHit?.Invoke(new ProjectileHit(point, -_direction, damageable, hitTarget, _damage));
            Release();
        }

        private void Release()
        {
            if (_trail != null)
            {
                _trail.emitting = false;
                _trail.Clear();
                _trail.enabled = false;
            }

            _active = false;
            _returnToPool?.Invoke(this);
        }
    }
}
