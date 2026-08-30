using Project.Configs;
using Project.Player;
using UnityEngine;
using VContainer;

namespace Project.CameraLogic
{
    public sealed class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        
        private CameraConfig _config;
        private PlayerController _player;
        private Vector3 _velocity;
        private float _appliedAspect;

        [Inject]
        public void Construct(GameConfig gameConfig, PlayerController player)
        {
            _config = gameConfig.Camera;
            _player = player;
            
            ApplyFieldOfView();
            SnapToTarget();
        }

        public void SnapToTarget()
        {
            if (!HasTarget())
                return;

            transform.position = _player.Transform.position + _config.Offset;
            transform.rotation = Quaternion.Euler(_config.FixedRotation);
            _velocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (!HasTarget())
                return;

            transform.position = Vector3.SmoothDamp(transform.position, _player.Transform.position + _config.Offset,
                ref _velocity,
                _config.FollowSmoothTime,
                _config.MaxFollowSpeed,
                Time.deltaTime);

            transform.rotation = Quaternion.Euler(_config.FixedRotation);

            ApplyFieldOfView();
        }

        private void ApplyFieldOfView()
        {
            if (_camera == null)
                return;

            var aspect = _camera.aspect;

            if (Mathf.Approximately(aspect, _appliedAspect))
                return;

            _appliedAspect = aspect;
            _camera.fieldOfView = ResolveFieldOfView(aspect);
        }

        private float ResolveFieldOfView(float aspect)
        {
            var reference = _config.ReferenceAspect;

            if (aspect <= 0f || aspect >= reference)
                return _config.FieldOfView;

            var halfHeight = Mathf.Tan(_config.FieldOfView * 0.5f * Mathf.Deg2Rad);
            var scaled = halfHeight * reference / aspect;

            return Mathf.Clamp(2f * Mathf.Atan(scaled) * Mathf.Rad2Deg, 1f, 179f);
        }

        private bool HasTarget()
        {
            return _config != null && _player != null;
        }
    }
}
