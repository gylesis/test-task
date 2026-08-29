using System;
using Project.Configs;
using Project.Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public sealed class EnemyHealthView : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _fill;

        private Health _health;
        private IDisposable _subscription;
        private bool _hideWhenFull;

        public void Bind(Health health, EnemyConfig config)
        {
            _subscription?.Dispose();

            _health = health;
            _hideWhenFull = config.HideHealthBarWhenFull;

            var position = _canvas.transform.localPosition;
            position.y = config.HealthBarHeight;
            _canvas.transform.localPosition = position;

            _subscription = _health.Current.Subscribe(_ => Refresh());
            Refresh();
        }

        public void Hide()
        {
            _canvas.enabled = false;
        }

        private void Refresh()
        {
            var normalized = _health.Normalized;
            
            _fill.fillAmount = normalized;
            _canvas.enabled = !_hideWhenFull || normalized < 1f;
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
