using Project.Configs;
using UnityEngine;

namespace Project.Input
{
    public sealed class MobileInputService : IInputService
    {
        private readonly InputConfig _config;
        private readonly IJoystick _joystick;

        public MobileInputService(GameConfig gameConfig, IJoystick joystick)
        {
            _config = gameConfig.Input;
            _joystick = joystick;
        }

        public Vector2 MoveDirection
        {
            get
            {
                if (_joystick == null)
                    return Vector2.zero;

                var raw = _joystick.Direction;
                var deadZone = _config.DeadZone;

                if (raw.sqrMagnitude <= deadZone * deadZone)
                    return Vector2.zero;

                var magnitude = Mathf.InverseLerp(deadZone, 1f, raw.magnitude);
                return raw.normalized * Mathf.Clamp01(magnitude);
            }
        }
    }
}
