using UnityEngine;

namespace Project.Configs
{
    public enum InputMode
    {
        Auto,
        Keyboard,
        Joystick
    }

    [CreateAssetMenu(fileName = "InputConfig", menuName = "Game/Input Config")]
    public sealed class InputConfig : ScriptableObject
    {
        [SerializeField] private InputMode _mode = InputMode.Auto;
        [SerializeField, Range(0f, 0.9f)] private float _deadZone = 0.15f;

        public float DeadZone => _deadZone;

        public InputMode ResolveMode()
        {
            if (_mode != InputMode.Auto)
                return _mode;

            return Application.isMobilePlatform ? InputMode.Joystick : InputMode.Keyboard;
        }
    }
}
