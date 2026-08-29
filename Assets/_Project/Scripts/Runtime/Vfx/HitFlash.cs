using System.Collections.Generic;
using UnityEngine;

namespace Project.Vfx
{
    public sealed class HitFlash : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        [SerializeField] private Color _flashColor = new(1f, 0.22f, 0.18f, 1f);
        [SerializeField, Min(0.01f)] private float _duration = 0.14f;

        private readonly List<Renderer> _renderers = new();
        private readonly List<Color> _originals = new();

        private MaterialPropertyBlock _block;
        private float _timeLeft;
        private bool _collected;

        public void Collect(GameObject model)
        {
            _renderers.Clear();
            _originals.Clear();

            if (model == null)
            {
                _collected = false;
                return;
            }

            _block ??= new MaterialPropertyBlock();
            model.GetComponentsInChildren(true, _renderers);

            for (var i = 0; i < _renderers.Count; i++)
            {
                var material = _renderers[i].sharedMaterial;
                var color = Color.white;

                if (material != null)
                {
                    if (material.HasProperty(BaseColorId))
                        color = material.GetColor(BaseColorId);
                    else if (material.HasProperty(ColorId))
                        color = material.GetColor(ColorId);
                }

                _originals.Add(color);
            }

            _collected = _renderers.Count > 0;
            Apply(0f);
        }

        public void Play()
        {
            if (!_collected)
                return;

            _timeLeft = _duration;
            Apply(1f);
        }

        public void Reset()
        {
            _timeLeft = 0f;
            Apply(0f);
        }

        private void Update()
        {
            if (_timeLeft <= 0f)
                return;

            _timeLeft -= Time.deltaTime;
            Apply(Mathf.Clamp01(_timeLeft / _duration));
        }

        private void Apply(float amount)
        {
            if (!_collected)
                return;

            for (var i = 0; i < _renderers.Count; i++)
            {
                var renderer = _renderers[i];

                if (renderer == null)
                    continue;

                var color = Color.Lerp(_originals[i], _flashColor, amount);

                renderer.GetPropertyBlock(_block);
                _block.SetColor(BaseColorId, color);
                _block.SetColor(ColorId, color);
                renderer.SetPropertyBlock(_block);
            }
        }
    }
}
