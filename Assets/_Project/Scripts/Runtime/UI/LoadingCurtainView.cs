using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public sealed class LoadingCurtainView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _progressFill;
        [SerializeField] private TMP_Text _stageLabel;
        [SerializeField] private TMP_Text _percentLabel;

        private IDisposable _subscription;

        public void Bind(IReadOnlyReactiveProperty<float> progress, IReadOnlyReactiveProperty<string> stage)
        {
            _subscription?.Dispose();

            _subscription = progress
                .CombineLatest(stage, (value, text) => (value, text))
                .Subscribe(data => Refresh(data.value, data.text))
                .AddTo(this);
        }

        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0f;

        public void ShowInstant()
        {
            SetAlpha(1f);
            SetInteractable(true);
        }

        public void HideInstant()
        {
            SetAlpha(0f);
            SetInteractable(false);
        }

        public IEnumerator FadeIn(float duration)
        {
            SetInteractable(true);
            yield return Fade(1f, duration);
        }

        public IEnumerator FadeOut(float duration)
        {
            yield return Fade(0f, duration);
            SetInteractable(false);
        }

        private IEnumerator Fade(float target, float duration)
        {
            if (_canvasGroup == null)
                yield break;

            var start = _canvasGroup.alpha;

            if (duration <= 0f || Mathf.Approximately(start, target))
            {
                SetAlpha(target);
                yield break;
            }

            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Lerp(start, target, elapsed / duration));
                yield return null;
            }

            SetAlpha(target);
        }

        private void Refresh(float value, string stage)
        {
            if (_progressFill != null)
                _progressFill.fillAmount = value;

            if (_stageLabel != null)
                _stageLabel.text = stage;

            if (_percentLabel != null)
                _percentLabel.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }

        private void SetAlpha(float alpha)
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = alpha;
        }

        private void SetInteractable(bool state)
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.blocksRaycasts = state;
            _canvasGroup.interactable = state;
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}
