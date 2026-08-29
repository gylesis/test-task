using UnityEngine;

namespace Project.Configs
{
    public enum PlayerRotationMode
    {
        TowardsTarget,
        TowardsMovement
    }

    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player Config")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [Header("View")]

        [Header("Aiming")]
        [SerializeField] private PlayerRotationMode _rotationMode = PlayerRotationMode.TowardsTarget;

        [Header("Stats")]
        [SerializeField, Min(1f)] private float _maxHealth = 100f;
        [SerializeField, Min(0f)] private float _moveSpeed = 6f;
        [SerializeField, Min(0f)] private float _rotationSpeed = 720f;
        [SerializeField] private float _gravity = -20f;

        [Header("Weapon")]
        [SerializeField] private WeaponConfig _weapon;

        public PlayerRotationMode RotationMode => _rotationMode;
        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;
        public float Gravity => _gravity;
        public WeaponConfig Weapon => _weapon;
    }
}
