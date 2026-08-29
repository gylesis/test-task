using DG.Tweening;
using UnityEngine;

namespace Project.CameraLogic
{
    [DefaultExecutionOrder(200)]
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.28f;
        [SerializeField] private float _strength = 0.45f;
        [SerializeField, Min(1)] private int _vibrato = 18;
        [SerializeField, Range(0f, 180f)] private float _randomness = 90f;
        [SerializeField, Range(0f, 1f)] private float _verticalFactor = 0.35f;
        [SerializeField] private float _rollFactor = 3f;
        [SerializeField] private bool _fadeOut = true;

        private Tween _tween;
        private Vector3 _offset;
        private Vector3 _applied;
        private float _currentStrength;

        public void Add(float scale)
        {
            if (scale <= 0f)
                return;

            var strength = _strength * scale;

            if (_tween != null && _tween.IsActive() && strength <= _currentStrength)
                return;

            Stop();

            _currentStrength = strength;

            _tween = DOTween.Shake(
                    () => _offset,
                    value => _offset = value,
                    _duration * Mathf.Clamp(scale, 0.5f, 1.5f),
                    strength,
                    _vibrato,
                    _randomness,
                    true,
                    _fadeOut)
                .SetTarget(this)
                .SetLink(gameObject)
                .OnComplete(() => _offset = Vector3.zero)
                .OnKill(() => _offset = Vector3.zero);
        }

        public void Stop()
        {
            if (_tween != null && _tween.IsActive())
                _tween.Kill();

            _tween = null;
            _offset = Vector3.zero;
            _currentStrength = 0f;
        }

        private void Update()
        {
            Revert();
        }

        private void LateUpdate()
        {
            if (_offset.sqrMagnitude <= 0.000001f)
                return;

            _applied = transform.right * _offset.x + transform.up * (_offset.y * _verticalFactor);

            transform.position += _applied;
            transform.rotation *= Quaternion.Euler(0f, 0f, _offset.x * _rollFactor);
        }

        private void Revert()
        {
            if (_applied.sqrMagnitude <= 0.000001f)
                return;

            transform.position -= _applied;
            _applied = Vector3.zero;
        }

        private void OnDestroy()
        {
            Stop();
        }
    }
}
