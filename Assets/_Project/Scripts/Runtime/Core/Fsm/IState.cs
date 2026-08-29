namespace Project.Core.Fsm
{
    public interface IState
    {
        void Enter();
        void Tick();
        void FixedTick();
        void Exit();
    }

    public interface IDrawGizmos
    {
        void DrawGizmos();
    }
}
