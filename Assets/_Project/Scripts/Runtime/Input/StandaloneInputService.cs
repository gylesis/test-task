using Project.Configs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Input
{
    public sealed class StandaloneInputService : IInputService
    {
        private readonly InputConfig _config;

        public StandaloneInputService(GameConfig gameConfig)
        {
            _config = gameConfig.Input;
        }

        public Vector2 MoveDirection
        {
            get
            {
                var keyboard = Keyboard.current;
                if (keyboard == null)
                    return Vector2.zero;

                var raw = Vector2.zero;

                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                    raw.x -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                    raw.x += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                    raw.y -= 1f;
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                    raw.y += 1f;

                if (raw.sqrMagnitude <= _config.DeadZone * _config.DeadZone)
                    return Vector2.zero;

                return Vector2.ClampMagnitude(raw, 1f);
            }
        }
    }
}
