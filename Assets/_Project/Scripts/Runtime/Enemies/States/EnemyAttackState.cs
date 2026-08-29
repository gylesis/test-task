using Project.Core.Fsm;
using UnityEngine;

namespace Project.Enemies.States
{
    public sealed class EnemyAttackState : EnemyState, IDrawGizmos
    {
        private float _cooldown;

        public EnemyAttackState(EnemyContext context) : base(context) { }

        public override void Enter()
        {
            _cooldown = 0f;
            Context.ReportMoveSpeed(0f);
        }

        public override void Tick()
        {
            if (!Context.HasPlayer)
            {
                Context.Controller.Machine.Enter<EnemyChaseState>();
                return;
            }

            var deltaTime = Time.deltaTime;
            var toPlayer = Context.ToPlayerFlat();

            if (toPlayer.magnitude > Context.Config.AttackRangeExit)
            {
                Context.Controller.Machine.Enter<EnemyChaseState>();
                return;
            }

            Context.RotateTowards(toPlayer, deltaTime);
            Context.Body.Move(Vector3.up * -9.81f * deltaTime);
            Context.ReportMoveSpeed(0f);

            _cooldown -= deltaTime;
            if (_cooldown > 0f)
                return;

            Context.Animator?.PlayAttack();
            Context.Player.TakeDamage(Context.Config.Damage.Roll());
            _cooldown = Context.Config.AttackInterval;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Context.Transform.position, Context.Config.AttackRange);
        }
    }
}
