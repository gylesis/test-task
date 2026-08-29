using System;
using System.Collections.Generic;
using Project.Animations;
using Project.Combat;
using Project.Configs;
using Project.Core;
using Project.Core.Fsm;
using Project.Player;
using Project.Ragdoll;
using Project.Enemies.States;
using Project.UI;
using Project.Vfx;
using UnityEngine;
using VContainer;

namespace Project.Enemies
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField] private GameObject _model;
        [SerializeField] private Transform _muzzlePoint;
        [SerializeField] private Ragdoll.Ragdoll _ragdoll;
        [SerializeField] private EnemyHealthView _healthView;
        [SerializeField] private Collider _hitCollider;
        [SerializeField] private EnemyAnimatorView _animatorView;
        [SerializeField] private HitFlash _hitFlash;

        private readonly Collider[] _neighbours = new Collider[8];

        private CharacterController _body;
        private EnemyRegistry _registry;
        private PlayerController _player;
        private GameStateService _gameState;
        private VfxController _vfx;
        private ProjectileService _projectiles;

        private EnemyConfig _config;
        private Health _health;
        private EnemyContext _context;
        private Action<EnemyController> _despawnHandler;
        private bool _initialized;

        public StateMachine<EnemyState> Machine { get; private set; }
        public EnemyConfig Config => _config;
        public bool IsAlive => _health != null && _health.IsAlive;
        public Transform Transform => transform;
        public EnemyAnimatorView AnimatorView => _animatorView;
        public Vector3 MuzzlePosition => _muzzlePoint != null
            ? _muzzlePoint.position
            : transform.position + Vector3.up * _config.MuzzleHeight;

        [Inject]
        public void Construct(
            EnemyRegistry registry,
            PlayerController player,
            GameStateService gameState,
            VfxController vfx,
            ProjectileService projectiles)
        {
            _projectiles = projectiles;
            _registry = registry;
            _player = player;
            _gameState = gameState;
            _vfx = vfx;
        }

        public void Initialize(EnemyConfig config, Action<EnemyController> despawnHandler)
        {
            _config = config;
            _despawnHandler = despawnHandler;
            _body = GetComponent<CharacterController>();

            _body.radius = config.Radius;
            _body.height = config.Height;
            _body.center = new Vector3(0f, config.Height * 0.5f, 0f);

            if (_hitCollider == null)
                _hitCollider = _body;
            _hitCollider.enabled = true;

            if (_health == null)
            {
                _health = new Health(config.MaxHealth);
                _health.Died += OnDied;
            }
            else
            {
                _health.Reset(config.MaxHealth);
            }

            if (!_initialized)
            {
                _hitFlash?.Collect(_model);
                _ragdoll?.Initialize();
                _initialized = true;
            }

            _healthView?.Bind(_health, config);

            _context = new EnemyContext(this, _body, config, _player, _registry, _health, _projectiles);
            Machine = new StateMachine<EnemyState>(EnemyStatesFactory.Build(_context));

            _registry.Register(this);
            Machine.Enter<EnemySpawnState>();
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive)
                return;

            _health.TakeDamage(amount);
            _hitFlash?.Play();
        }

        public Vector3 CalculateSeparation()
        {
            var radius = _config.SeparationRadius;
            if (radius <= 0f)
                return Vector3.zero;

            var count = Physics.OverlapSphereNonAlloc(transform.position, radius, _neighbours, 1 << gameObject.layer, QueryTriggerInteraction.Ignore);
            var result = Vector3.zero;

            for (var i = 0; i < count; i++)
            {
                var other = _neighbours[i];
                if (other == null || other.transform == transform || other.GetComponentInParent<EnemyController>() == null)
                    continue;

                var delta = transform.position - other.transform.position;
                delta.y = 0f;

                var distance = delta.magnitude;
                if (distance <= 0.0001f)
                {
                    result += UnityEngine.Random.insideUnitSphere.normalized;
                    continue;
                }

                result += delta / distance * (1f - Mathf.Clamp01(distance / radius));
            }

            return result;
        }

        public void OnDeathEnter()
        {
            if (_hitCollider != null)
                _hitCollider.enabled = false;

            _healthView?.Hide();
            _vfx?.Play(VfxIds.EnemyDeath, transform.position + Vector3.up * (_config.Height * 0.5f));

            if (_config.UseRagdoll && _ragdoll != null)
                ActivateRagdoll();
            else
                _animatorView?.PlayDeath();
        }

        private void ActivateRagdoll()
        {
            var direction = transform.forward;

            if (_player != null)
            {
                var away = transform.position - _player.transform.position;
                away.y = 0f;

                if (away.sqrMagnitude > 0.0001f)
                    direction = away.normalized;
            }

            var impulse = direction * _config.DeathImpulse.Roll() + Vector3.up * _config.DeathImpulseUp;
            var point = transform.position + Vector3.up * (_config.Height * 0.6f);

            _ragdoll.Activate(impulse, point);
        }

        public void Despawn()
        {
            _despawnHandler?.Invoke(this);
        }

        public void ResetForPool()
        {
            Machine?.Exit();
            _hitFlash?.Reset();
            _ragdoll?.Restore();
            _registry.Unregister(this);
        }

        private void Update()
        {
            if (Machine == null || !_gameState.IsRunning)
                return;

            Machine.Tick();
        }

        private void FixedUpdate()
        {
            if (Machine == null || !_gameState.IsRunning)
                return;

            Machine.FixedTick();
        }

        private void OnDied()
        {
            Machine?.Enter<EnemyDeathState>();
        }

        private void OnDrawGizmosSelected()
        {
            Machine?.DrawGizmos();
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.Died -= OnDied;
        }
    }
}
