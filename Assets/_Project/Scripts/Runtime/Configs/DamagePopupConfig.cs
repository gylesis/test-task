using Project.UI;
using UnityEngine;

namespace Project.Configs
{
    [CreateAssetMenu(fileName = "DamagePopupConfig", menuName = "Game/Damage Popup Config")]
    public sealed class DamagePopupConfig : ScriptableObject
    {
        [Header("Prefab")]
        [SerializeField] private DamagePopupView _prefab;
        [SerializeField, Min(1)] private int _poolSize = 24;

        [Header("Placement")]
        [SerializeField] private float _heightOffset = 1.8f;
        [SerializeField] private Vector2 _randomOffset = new(0.35f, 0.25f);

        [Header("Animation")]
        [SerializeField, Min(0.05f)] private float _duration = 0.8f;
        [SerializeField] private float _riseDistance = 1.2f;
        [SerializeField, Min(0.0001f)] private float _worldScale = 0.01f;
        [SerializeField] private AnimationCurve _riseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _alphaCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1f);

        [Header("Look")]
        [SerializeField] private Color _color = new(1f, 0.86f, 0.35f, 1f);
        [SerializeField] private string _format = "0";

        public DamagePopupView Prefab => _prefab;
        public int PoolSize => _poolSize;
        public float HeightOffset => _heightOffset;
        public Vector2 RandomOffset => _randomOffset;
        public float Duration => _duration;
        public float RiseDistance => _riseDistance;
        public float WorldScale => _worldScale;
        public AnimationCurve RiseCurve => _riseCurve;
        public AnimationCurve AlphaCurve => _alphaCurve;
        public AnimationCurve ScaleCurve => _scaleCurve;
        public Color Color => _color;
        public string Format => _format;
    }
}
