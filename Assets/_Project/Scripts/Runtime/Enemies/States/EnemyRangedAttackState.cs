using Project.Combat;
using Project.Core.Fsm;
using UnityEngine;

namespace Project.Enemies.States
{
    public sealed class EnemyRangedAttackState : EnemyState, IDrawGizmos
    {
        private float _cooldown;

        public EnemyRangedAttackState(EnemyContext context) : base(context) { }

        public override void Enter()
        {
            _cooldown = Mathf.Min(_cooldown, Context.Config.AimDelay);
        }

        public override void Tick()
        {
            if (!Context.HasPlayer)
            {
                Context.Controller.Machine.Enter<EnemyChaseState>();
                return;
            }

            var config = Context.Config;
            var deltaTime = Time.deltaTime;
            var toPlayer = Context.ToPlayerFlat();
            var distance = toPlayer.magnitude;

            if (distance > config.AttackRangeExit)
            {
                Context.Controller.Machine.Enter<EnemyChaseState>();
                return;
            }

            var forward = distance > 0.0001f ? toPlayer / distance : Vector3.zero;
            Context.RotateTowards(forward, deltaTime);

            if (distance < config.KeepDistance)
                Retreat(forward, config.RetreatSpeed, deltaTime);
            else
                Hold(deltaTime);

            _cooldown -= deltaTime;
            if (_cooldown > 0f)
                return;

            Context.Animator?.PlayAttack();
            Shoot(config);
            _cooldown = config.AttackInterval;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Context.Transform.position, Context.Config.AttackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Context.Transform.position, Context.Config.KeepDistance);
        }

        private void Retreat(Vector3 forward, float speed, float deltaTime)
        {
            var steering = -forward + Context.Controller.CalculateSeparation() * Context.Config.SeparationWeight;

            if (steering.sqrMagnitude > 0.0001f)
                steering.Normalize();

            Context.Body.Move((steering * speed + Vector3.up * -9.81f) * deltaTime);
            Context.ReportMoveSpeed(speed);
        }

        private void Hold(float deltaTime)
        {
            Context.Body.Move(Vector3.up * -9.81f * deltaTime);
            Context.ReportMoveSpeed(0f);
        }

        private void Shoot(Configs.EnemyConfig config)
        {
            var muzzle = Context.Controller.MuzzlePosition;
            var direction = Context.Player.transform.position - muzzle;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f)
                return;

            Context.Projectiles.Spawn(new ProjectileRequest(
                config.ProjectilePrefab,
                muzzle,
                direction.normalized,
                config.ProjectileSpeed,
                config.Damage.Roll(),
                config.ProjectileLifetime,
                config.ProjectileRadius,
                config.HitMask,
                config.ObstacleMask));
        }
    }
}
