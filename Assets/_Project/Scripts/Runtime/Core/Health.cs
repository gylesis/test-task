using System;
using UniRx;
using UnityEngine;

namespace Project.Core
{
    public sealed class Health
    {
        private readonly ReactiveProperty<float> _current = new();

        public Health(float max)
        {
            Max = max;
            _current.Value = max;
        }

        public float Max { get; private set; }
        public IReadOnlyReactiveProperty<float> Current => _current;
        public float Normalized => Max <= 0f ? 0f : Mathf.Clamp01(_current.Value / Max);
        public bool IsAlive => _current.Value > 0f;
        public bool IsInvulnerable { get; set; }

        public event Action Died;
        public event Action<float> Damaged;

        public void Reset(float max)
        {
            Max = max;
            IsInvulnerable = false;
            _current.Value = max;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || IsInvulnerable || amount <= 0f)
                return;

            _current.Value = Mathf.Max(0f, _current.Value - amount);
            Damaged?.Invoke(amount);

            if (_current.Value <= 0f)
                Died?.Invoke();
        }
    }
}
