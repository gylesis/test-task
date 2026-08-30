using DG.Tweening;
using Project.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;

namespace Project.UI
{
    [RequireComponent(typeof(Volume))]
    public sealed class DamageVignetteView : MonoBehaviour
    {
        [SerializeField] private Color _color = new(0.75f, 0.05f, 0.05f, 1f);
        [SerializeField, Range(0f, 1f)] private float _intensity = 0.55f;
        [SerializeField, Range(0f, 1f)] private float _smoothness = 0.5f;
        [SerializeField, Min(0f)] private float _fadeIn = 0.06f;
        [SerializeField, Min(0f)] private float _fadeOut = 0.35f;
        [SerializeField] private int _priority = 10;

        private Volume _volume;
        private PlayerController _player;
        private Tween _tween;

        [Inject]
        public void Construct(PlayerController player)
        {
            _player = player;
            _player.Health.Damaged += OnDamaged;
        }

        private void Awake()
        {
            _volume = GetComponent<Volume>();
            _volume.isGlobal = true;
            _volume.priority = _priority;
            _volume.weight = 0f;
            _volume.profile = CreateProfile();
        }

        private VolumeProfile CreateProfile()
        {
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            var vignette = profile.Add<Vignette>();

            vignette.active = true;

            vignette.color.overrideState = true;
            vignette.color.value = _color;

            vignette.intensity.overrideState = true;
            vignette.intensity.value = _intensity;

            vignette.smoothness.overrideState = true;
            vignette.smoothness.value = _smoothness;

            return profile;
        }

        private void OnDamaged(float amount)
        {
            if (_volume == null)
                return;

            _tween?.Kill();

            _tween = DOTween.Sequence()
                .Append(DOTween.To(() => _volume.weight, value => _volume.weight = value, 1f, _fadeIn))
                .Append(DOTween.To(() => _volume.weight, value => _volume.weight = value, 0f, _fadeOut))
                .SetTarget(this)
                .SetLink(gameObject);
        }

        private void OnDestroy()
        {
            _tween?.Kill();

            if (_player != null && _player.Health != null)
                _player.Health.Damaged -= OnDamaged;

            if (_volume != null && _volume.profile != null)
                Destroy(_volume.profile);
        }
    }
}
