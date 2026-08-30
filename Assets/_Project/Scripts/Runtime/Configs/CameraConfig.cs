using UnityEngine;

namespace Project.Configs
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Game/Camera Config")]
    public sealed class CameraConfig : ScriptableObject
    {
        [SerializeField] private Vector3 _offset = new(0f, 16f, -9f);
        [SerializeField, Min(0f)] private float _followSmoothTime = 0.2f;
        [SerializeField, Min(0f)] private float _maxFollowSpeed = 60f;
        [SerializeField, Range(20f, 90f)] private float _fieldOfView = 55f;
        [SerializeField, Min(0.5f)] private float _referenceAspect = 16f / 9f;
        [SerializeField] private Vector3 _fixedRotation = new(57f, 0f, 0f);

        public Vector3 Offset => _offset;
        public float FollowSmoothTime => _followSmoothTime;
        public float MaxFollowSpeed => _maxFollowSpeed;
        public float FieldOfView => _fieldOfView;
        public float ReferenceAspect => _referenceAspect;
        public Vector3 FixedRotation => _fixedRotation;
    }
}
