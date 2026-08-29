using UnityEngine;

namespace Project.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField] private PlayerConfig _player;
        [SerializeField] private SpawnConfig _spawn;
        [SerializeField] private CameraConfig _camera;
        [SerializeField] private InputConfig _input;
        [SerializeField] private VfxConfig _vfx;
        [SerializeField] private DamagePopupConfig _damagePopup;

        public PlayerConfig Player => _player;
        public SpawnConfig Spawn => _spawn;
        public CameraConfig Camera => _camera;
        public InputConfig Input => _input;
        public VfxConfig Vfx => _vfx;
        public DamagePopupConfig DamagePopup => _damagePopup;
    }
}
