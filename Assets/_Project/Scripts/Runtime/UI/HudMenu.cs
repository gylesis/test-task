using System;
using Project.Configs;
using Project.Core;
using Project.Infrastructure;
using UniRx;
using UnityEngine;
using VContainer;

namespace Project.UI
{
    public sealed class HudMenu : Menu
    {
        [SerializeField] private ButtonView _restartButton;

        private SceneLoader _sceneLoader;
        private IDisposable _subscription;

        [Inject]
        public void Construct(GameStateService gameState, SceneLoader sceneLoader, GameConfig gameConfig)
        {
            _sceneLoader = sceneLoader;

            _restartButton.Clicked += Restart;

            SetJoystickActive(gameConfig.Input.ResolveMode() == InputMode.Joystick);

            _subscription = gameState.State
                .Subscribe(state => SetShown(state != GameState.GameOver))
                .AddTo(this);
        }

        private void Restart()
        {
            if (_sceneLoader.IsLoading)
                return;
            
            _restartButton.Interactable = false;
            _sceneLoader.Reload();
        }

        private void SetJoystickActive(bool active)
        {
            foreach (var joystick in GetComponentsInChildren<JoystickView>(true))
                joystick.gameObject.SetActive(active);
        }

        private void OnDestroy()
        {
            _restartButton.Clicked -= Restart;
            _subscription?.Dispose();
        }
    }
}
