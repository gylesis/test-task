using System;
using Project.Core;
using Project.Infrastructure;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace Project.UI
{
    public sealed class GameOverMenu : Menu
    {
        [SerializeField] private TMP_Text _resultLabel;
        [SerializeField] private ButtonView _restartButton;

        private GameStateService _gameState;
        private SceneLoader _sceneLoader;
        private IDisposable _subscription;

        [Inject]
        public void Construct(GameStateService gameState, SceneLoader sceneLoader)
        {
            _gameState = gameState;

            _sceneLoader = sceneLoader;

            _restartButton.Clicked += Restart;

            _subscription = gameState.State
                .Subscribe(state => SetShown(state == GameState.GameOver))
                .AddTo(this);
        }

        public override void Show()
        {
            base.Show();
            
            _restartButton.Interactable = true;
            
            var span = TimeSpan.FromSeconds(_gameState.Elapsed.Value);
            _resultLabel.text = $"Продержался {span.Minutes:00}:{span.Seconds:00}";
        }

        private void Restart()
        {
            if (_sceneLoader.IsLoading)
                return;
            
            _restartButton.Interactable = false;
            _sceneLoader.Reload();
        }

        private void OnDestroy()
        {
            _restartButton.Clicked -= Restart;
            _subscription?.Dispose();
        }
    }
}
