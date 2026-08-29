using System;
using Project.Core;
using Project.Player;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.UI
{
    public sealed class PlayerHealthView : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private float _lerpSpeed = 8f;

        private Health _health;
        private IDisposable _subscription;
        private float _displayed = 1f;

        [Inject]
        public void Construct(PlayerController player)
        {
            _health = player.Health;
            _displayed = 1f;

            _subscription = _health.Current
                .Subscribe(_ => Refresh())
                .AddTo(this);
        }

        private void Update()
        {
            _fill.fillAmount = Mathf.MoveTowards(_fill.fillAmount, _displayed, _lerpSpeed * Time.deltaTime);
        }

        private void Refresh()
        {
            _displayed = _health.Normalized;
            _label.text = $"{Mathf.CeilToInt(_health.Current.Value)} / {Mathf.CeilToInt(_health.Max)}";
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
