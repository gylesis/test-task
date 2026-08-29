using Project.CameraLogic;
using Project.Combat;
using Project.Configs;
using Project.Core;
using Project.Enemies;
using Project.Input;
using Project.Player;
using Project.UI;
using Project.Vfx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Infrastructure
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Config")]
        [SerializeField] private GameConfig _gameConfig;

        [Header("Scene")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private VfxController _vfx;
        [SerializeField] private MonoBehaviour _joystickSource;

        [Header("Prefabs")]
        [SerializeField] private RootLifetimeScope _rootScopePrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigs(builder);
            RegisterInput(builder);
            RegisterSceneComponents(builder);

            builder.RegisterBuildCallback(ResolveSceneComponents);

            RegisterServices(builder);
        }

        protected override LifetimeScope FindParent()
        {
            var existing = Find<RootLifetimeScope>();
            if (existing != null)
                return existing;

            return _rootScopePrefab != null ? Instantiate(_rootScopePrefab) : null;
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(_gameConfig);
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<EnemyRegistry>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameStateService>().AsSelf();
            builder.Register<ProjectileService>(Lifetime.Singleton);
            builder.Register<DamagePopupService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<EnemyFactory>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameBootstrap>();
            builder.RegisterEntryPoint<EnemySpawner>();
        }

        private void RegisterInput(IContainerBuilder builder)
        {
            RegisterJoystick(builder);

            if (_gameConfig.Input.ResolveMode() == InputMode.Joystick)
            {
                builder.Register<MobileInputService>(Lifetime.Singleton).As<IInputService>();
                return;
            }

            builder.Register<StandaloneInputService>(Lifetime.Singleton).As<IInputService>();
        }

        private void RegisterJoystick(IContainerBuilder builder)
        {
            if (_joystickSource is IJoystick joystick)
            {
                builder.RegisterInstance(joystick).As<IJoystick>();
                return;
            }

            builder.RegisterComponentInHierarchy<JoystickView>().As<IJoystick>();
        }

        private void RegisterSceneComponents(IContainerBuilder builder)
        {
            builder.RegisterComponent(_player).AsSelf().As<IDamageable>();
            builder.RegisterComponent(_vfx);
            builder.RegisterComponentInHierarchy<CameraController>();
            builder.RegisterComponentInHierarchy<CameraShake>();
            builder.RegisterComponentInHierarchy<PlayerHealthView>();
            builder.RegisterComponentInHierarchy<SurvivalTimerView>();
            builder.RegisterComponentInHierarchy<HudMenu>();
            builder.RegisterComponentInHierarchy<GameOverMenu>();
        }

        private static void ResolveSceneComponents(IObjectResolver container)
        {
            container.Resolve<PlayerController>();
            container.Resolve<VfxController>();
            container.Resolve<CameraController>();
            container.Resolve<PlayerHealthView>();
            container.Resolve<SurvivalTimerView>();
            container.Resolve<HudMenu>();
            container.Resolve<GameOverMenu>();
        }
    }
}
