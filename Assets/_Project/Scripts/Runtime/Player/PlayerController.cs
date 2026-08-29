using Project.Animations;
using Project.CameraLogic;
using Project.Combat;
using Project.Configs;
using Project.Core;
using Project.Input;
using Project.Vfx;
using UnityEngine;
using VContainer;

namespace Project.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour, IDamageable
    {
        [SerializeField] private Transform _modelRoot;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerWeapon _weapon;
        [SerializeField] private PlayerAnimatorView _animatorView;

        private PlayerConfig _playerConfig;
        private GameStateService _gameState;
        private VfxController _vfx;
        private CameraShake _shake;
        private Health _health;

        public Health Health => _health;
        public bool IsAlive => _health != null && _health.IsAlive;
        public Transform Transform => transform;

        [Inject]
        public void Construct(GameConfig gameConfig, IInputService inputService, EnemyRegistry enemies, ProjectileService projectiles, GameStateService gameState, VfxController vfx, CameraShake shake)
        {
            _shake = shake;
            _playerConfig = gameConfig.Player;
            _gameState = gameState;
            _vfx = vfx;

            _health = new Health(_playerConfig.MaxHealth);
            _health.Died += OnDied;

            _movement.Initialize(_playerConfig, inputService);
            _weapon.Initialize(_playerConfig.Weapon, enemies, projectiles, vfx);

            _weapon.Fired += OnWeaponFired;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive)
                return;

            _health.TakeDamage(amount);
            _vfx?.Play(VfxIds.PlayerHit, transform.position + Vector3.up);
            _shake?.Add(0.2f);
        }

        private void Update()
        {
            if (!_gameState.IsRunning || !IsAlive)
                return;

            var deltaTime = Time.deltaTime;
            _weapon.Tick(deltaTime);

            var aimAtTarget = _playerConfig.RotationMode == PlayerRotationMode.TowardsTarget
                              && _weapon.CurrentTarget != null;

            _movement.Tick(deltaTime, !aimAtTarget);

            if (aimAtTarget)
                _movement.RotateTowards(_weapon.GetAimDirection(), deltaTime);

            if (_animatorView != null)
                _animatorView.SetSpeed(Mathf.Clamp01(_movement.Velocity.magnitude / Mathf.Max(0.01f, _playerConfig.MoveSpeed)));
        }

        private void OnWeaponFired()
        {
            _animatorView?.PlayShoot();
        }

        private void OnDied()
        {
            _animatorView?.PlayDeath();
            _shake?.Add(0.5f);
            _gameState.SetGameOver();
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.Died -= OnDied;

            if (_weapon != null)
                _weapon.Fired -= OnWeaponFired;
        }
    }
}
