using Project.Configs;
using Project.Input;
using UnityEngine;

namespace Project.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        private CharacterController _controller;
        private PlayerConfig _config;
        private IInputService _inputService;
        private float _verticalVelocity;

        public Vector3 Velocity { get; private set; }

        public void Initialize(PlayerConfig config, IInputService input)
        {
            _config = config;
            _inputService = input;
            _controller = GetComponent<CharacterController>();
        }

        public void Tick(float deltaTime, bool canRotate)
        {
            var raw = _inputService.MoveDirection;
            var direction = new Vector3(raw.x, 0f, raw.y);

            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            _verticalVelocity += _config.Gravity * deltaTime;

            var planar = direction * _config.MoveSpeed;
            Velocity = planar;

            var motion = planar + Vector3.up * _verticalVelocity;
            _controller.Move(motion * deltaTime);

            if (canRotate && planar.sqrMagnitude > 0.0001f)
                RotateTowards(planar, deltaTime);
        }

        public void RotateTowards(Vector3 direction, float deltaTime)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            var target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, _config.RotationSpeed * deltaTime);
        }
    }
}
