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

        [Inject]
        public void Construct(GameConfig gameConfig, PlayerController player)
        {
            _config = gameConfig.Camera;
            _player = player;
            
            _camera.fieldOfView = _config.FieldOfView;
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
        }

        private bool HasTarget()
        {
            return _config != null && _player != null;
        }
    }
}
