using System;
using Project.Core;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace Project.UI
{
    public sealed class SurvivalTimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private IDisposable _subscription;

        [Inject]
        public void Construct(GameStateService gameState)
        {
            _subscription = gameState.Elapsed
                .Subscribe(Refresh)
                .AddTo(this);
        }

        private void Refresh(float elapsed)
        {
            if (_label == null)
                return;

            var span = TimeSpan.FromSeconds(elapsed);
            _label.text = $"{span.Minutes:00}:{span.Seconds:00}";
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
