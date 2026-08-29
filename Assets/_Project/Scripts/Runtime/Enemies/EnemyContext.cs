using Project.Animations;
using Project.Combat;
using Project.Configs;
using Project.Core;
using Project.Player;
using UnityEngine;

namespace Project.Enemies
{
    public sealed class EnemyContext
    {
        public EnemyContext(
            EnemyController controller,
            CharacterController body,
            EnemyConfig config,
            PlayerController player,
            EnemyRegistry registry,
            Health health,
            ProjectileService projectiles)
        {
            Controller = controller;
            Body = body;
            Config = config;
            Player = player;
            Registry = registry;
            Health = health;
            Projectiles = projectiles;
            Transform = controller.transform;
        }

        public EnemyController Controller { get; }
        public CharacterController Body { get; }
        public EnemyConfig Config { get; }
        public PlayerController Player { get; }
        public EnemyRegistry Registry { get; }
        public Health Health { get; }
        public ProjectileService Projectiles { get; }
        public Transform Transform { get; }
        public EnemyAnimatorView Animator => Controller.AnimatorView;

        public void ReportMoveSpeed(float speed)
        {
            var reference = Mathf.Max(0.01f, Config.AnimationSpeedReference);
            Animator?.SetSpeed(Mathf.Clamp01(speed / reference));
        }

        public void EnterAttackState()
        {
            EnemyStatesFactory.EnterCombat(this);
        }

        public bool HasPlayer => Player != null && Player.IsAlive;

        public Vector3 ToPlayerFlat()
        {
            if (!HasPlayer)
                return Vector3.zero;

            var delta = Player.Transform.position - Transform.position;
            delta.y = 0f;
            return delta;
        }

        public float DistanceToPlayer()
        {
            return HasPlayer ? ToPlayerFlat().magnitude : float.MaxValue;
        }

        public void RotateTowards(Vector3 direction, float deltaTime)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            var target = Quaternion.LookRotation(direction, Vector3.up);
            Transform.rotation = Quaternion.RotateTowards(Transform.rotation, target, Config.RotationSpeed * deltaTime);
        }
    }
}
