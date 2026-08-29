using UnityEngine;

namespace Project.Enemies.States
{
    public sealed class EnemySpawnState : EnemyState
    {
        private float _timeLeft;

        public EnemySpawnState(EnemyContext context) : base(context) { }

        public override void Enter()
        {
            _timeLeft = Context.Config.SpawnDelay;
            Context.Health.IsInvulnerable = true;
            Context.Animator?.PlaySpawn();
        }

        public override void Tick()
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f)
                Context.Controller.Machine.Enter<EnemyChaseState>();
        }

        public override void Exit()
        {
            Context.Health.IsInvulnerable = false;
        }
    }
}
