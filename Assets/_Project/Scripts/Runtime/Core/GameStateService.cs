using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Project.Core
{
    public sealed class GameStateService : ITickable
    {
        private readonly ReactiveProperty<GameState> _state = new(GameState.Loading);
        private readonly ReactiveProperty<float> _elapsed = new();

        public IReadOnlyReactiveProperty<GameState> State => _state;
        public IReadOnlyReactiveProperty<float> Elapsed => _elapsed;
        public bool IsRunning => _state.Value == GameState.Running;

        public void SetRunning()
        {
            _elapsed.Value = 0f;
            _state.Value = GameState.Running;
        }

        public void SetGameOver()
        {
            _state.Value = GameState.GameOver;
        }

        public void Tick()
        {
            if (IsRunning)
                _elapsed.Value += Time.deltaTime;
        }
    }
}
