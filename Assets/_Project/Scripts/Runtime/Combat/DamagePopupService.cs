using System;
using System.Globalization;
using Project.Configs;
using Project.UI;
using UnityEngine;
using UnityEngine.Pool;
using VContainer.Unity;

namespace Project.Combat
{
    public sealed class DamagePopupService : IDisposable, IInitializable
    {
        private DamagePopupConfig _config;
        private ObjectPool<DamagePopupView> _pool;
        private Transform _root;
        private PopupAnimation _animation;

        public DamagePopupService(GameConfig gameConfig)
        {
            var config = gameConfig.DamagePopup;
            _config = config;
        }

        public void Initialize()
        {
            _root = new GameObject("[DamagePopups]").transform;

            _animation = new PopupAnimation(
                _config.Duration,
                _config.RiseDistance,
                _config.WorldScale,
                _config.RiseCurve,
                _config.AlphaCurve,
                _config.ScaleCurve);

            _pool = new ObjectPool<DamagePopupView>(
                Create,
                popup => SetActiveSafe(popup, true),
                popup => SetActiveSafe(popup, false),
                DestroySafe,
                false,
                _config.PoolSize,
                _config.PoolSize * 4);
        }

        public void Show(float amount, Vector3 worldPosition)
        {
            if (_config.Prefab == null || amount <= 0f)
                return;

            var offset = _config.RandomOffset;
            var position = worldPosition + new Vector3(
                UnityEngine.Random.Range(-offset.x, offset.x),
                _config.HeightOffset + UnityEngine.Random.Range(-offset.y, offset.y),
                UnityEngine.Random.Range(-offset.x, offset.x));

            var text = amount.ToString(_config.Format, CultureInfo.InvariantCulture);
            _pool.Get().Show(text, _config.Color, position, _animation);
        }

        public void Prewarm(int count)
        {
            if (_config.Prefab == null || count <= 0)
                return;

            var buffer = new DamagePopupView[count];

            for (var i = 0; i < count; i++)
                buffer[i] = _pool.Get();

            for (var i = 0; i < count; i++)
                _pool.Release(buffer[i]);
        }

        public void Dispose()
        {
            _pool.Dispose();

            if (_root != null)
                UnityEngine.Object.Destroy(_root.gameObject);
        }

        private DamagePopupView Create()
        {
            var instance = UnityEngine.Object.Instantiate(_config.Prefab, _root);
            instance.Finished += Release;
            return instance;
        }

        private void Release(DamagePopupView popup)
        {
            if (popup != null && popup.gameObject.activeSelf)
                _pool.Release(popup);
        }

        private static void SetActiveSafe(DamagePopupView popup, bool state)
        {
            if (popup != null)
                popup.gameObject.SetActive(state);
        }

        private static void DestroySafe(DamagePopupView popup)
        {
            if (popup != null)
                UnityEngine.Object.Destroy(popup.gameObject);
        }
    }
}
