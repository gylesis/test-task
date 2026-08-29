using System;
using TMPro;
using UnityEngine;

namespace Project.UI
{
    public sealed class DamagePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Vector3 _origin;
        private float _elapsed;
        private float _duration;
        private float _riseDistance;
        private AnimationCurve _riseCurve;
        private AnimationCurve _alphaCurve;
        private AnimationCurve _scaleCurve;
        private Transform _cameraTransform;
        private Vector3 _baseScale;
        private bool _playing;

        public event Action<DamagePopupView> Finished;

        public void Show(string text, Color color, Vector3 worldPosition, PopupAnimation animation)
        {
            _origin = worldPosition;
            _elapsed = 0f;
            _duration = Mathf.Max(0.05f, animation.Duration);
            _riseDistance = animation.RiseDistance;
            _riseCurve = animation.Rise;
            _alphaCurve = animation.Alpha;
            _scaleCurve = animation.Scale;
            _baseScale = Vector3.one * Mathf.Max(0.0001f, animation.WorldScale);
            _playing = true;

            if (_label != null)
            {
                _label.text = text;
                _label.color = color;
            }

            transform.position = _origin;
            Apply(0f);
        }

        private void LateUpdate()
        {
            if (!_playing)
                return;

            _elapsed += Time.deltaTime;
            var progress = Mathf.Clamp01(_elapsed / _duration);

            Apply(progress);
            FaceCamera();

            if (progress >= 1f)
            {
                _playing = false;
                Finished?.Invoke(this);
            }
        }

        private void Apply(float progress)
        {
            var rise = _riseCurve?.Evaluate(progress) ?? progress;
            transform.position = _origin + Vector3.up * (rise * _riseDistance);

            var scale = _scaleCurve?.Evaluate(progress) ?? 1f;
            transform.localScale = _baseScale * scale;

            if (_canvasGroup != null)
                _canvasGroup.alpha = _alphaCurve?.Evaluate(progress) ?? 1f;
        }

        private void FaceCamera()
        {
            if (_cameraTransform == null)
            {
                var main = Camera.main;
                if (main == null)
                    return;

                _cameraTransform = main.transform;
            }

            transform.rotation = Quaternion.LookRotation(_cameraTransform.forward, Vector3.up);
        }
    }

    public readonly struct PopupAnimation
    {
        public PopupAnimation(
            float duration,
            float riseDistance,
            float worldScale,
            AnimationCurve rise,
            AnimationCurve alpha,
            AnimationCurve scale)
        {
            Duration = duration;
            RiseDistance = riseDistance;
            WorldScale = worldScale;
            Rise = rise;
            Alpha = alpha;
            Scale = scale;
        }

        public float Duration { get; }
        public float RiseDistance { get; }
        public float WorldScale { get; }
        public AnimationCurve Rise { get; }
        public AnimationCurve Alpha { get; }
        public AnimationCurve Scale { get; }
    }
}
