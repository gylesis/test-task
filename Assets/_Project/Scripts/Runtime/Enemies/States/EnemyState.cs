using Project.Core.Fsm;

namespace Project.Enemies.States
{
    public abstract class EnemyState : IState
    {
        protected readonly EnemyContext Context;

        protected EnemyState(EnemyContext context)
        {
            Context = context;
        }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void FixedTick() { }
        public virtual void Exit() { }
    }
}
