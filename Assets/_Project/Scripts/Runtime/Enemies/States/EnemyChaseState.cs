using Project.Core.Fsm;
using UnityEngine;

namespace Project.Enemies.States
{
    public sealed class EnemyChaseState : EnemyState, IDrawGizmos
    {
        public EnemyChaseState(EnemyContext context) : base(context) { }

        public override void Tick()
        {
            if (!Context.HasPlayer)
            {
                Context.ReportMoveSpeed(0f);
                return;
            }

            var toPlayer = Context.ToPlayerFlat();
            var distance = toPlayer.magnitude;

            var config = Context.Config;
            var deltaTime = Time.deltaTime;
            var forward = distance > 0.0001f ? toPlayer / distance : Vector3.zero;

            if (distance <= config.AttackRange)
            {
                Context.EnterAttackState();
                return;
            }

            var steering = forward + Context.Controller.CalculateSeparation() * config.SeparationWeight;

            if (steering.sqrMagnitude > 0.0001f)
                steering.Normalize();

            var motion = steering * config.MoveSpeed + Vector3.up * -9.81f;
            Context.Body.Move(motion * deltaTime);
            Context.RotateTowards(forward, deltaTime);
            Context.ReportMoveSpeed(config.MoveSpeed);
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Context.Transform.position, Context.Config.AttackRange);

            if (!Context.HasPlayer)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Context.Transform.position, Context.Player.transform.position);
        }
    }
}
