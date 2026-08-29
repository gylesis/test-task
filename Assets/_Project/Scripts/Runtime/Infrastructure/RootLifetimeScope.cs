using Project.Configs;
using Project.Core;
using Project.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Infrastructure
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [Header("Config")]
        [SerializeField] private SceneLoadingConfig _sceneLoadingConfig;

        [Header("Startup")]
        [SerializeField, Min(30)] private int _targetFrameRate = 60;

        [Header("Scene")]
        [SerializeField] private LoadingCurtainView _curtain;
        [SerializeField] private CoroutineRunner _coroutineRunner;

        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = _targetFrameRate;
            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            builder.RegisterInstance(_sceneLoadingConfig);
            builder.RegisterComponent(_curtain);
            builder.RegisterComponent(_coroutineRunner);

            builder.Register<SceneLoader>(Lifetime.Singleton);

            builder.RegisterBuildCallback(container => container.Resolve<SceneLoader>());
            
            builder.RegisterEntryPoint<BootstrapEntryPoint>();
        }
    }
}
