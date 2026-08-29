using UnityEngine;

namespace Project.Enemies.States
{
    public sealed class EnemyDeathState : EnemyState
    {
        private float _timeLeft;

        public EnemyDeathState(EnemyContext context) : base(context) { }

        public override void Enter()
        {
            _timeLeft = Context.Config.DeathDelay;
            Context.Registry.Unregister(Context.Controller);
            Context.Controller.OnDeathEnter();
        }

        public override void Tick()
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f)
                Context.Controller.Despawn();
        }
    }
}
