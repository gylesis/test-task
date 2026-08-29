using System;
using System.Collections;
using Project.Configs;
using Project.Core;
using Project.UI;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Infrastructure
{
    public sealed class SceneLoader
    {
        private const float SceneLoadPortion = 0.7f;

        private readonly SceneLoadingConfig _config;
        private readonly LoadingCurtainView _curtain;
        private readonly ReactiveProperty<float> _progress = new();
        private readonly ReactiveProperty<string> _stage = new(string.Empty);
        private readonly CoroutineRunner _runner;
        private bool _sceneReady;
        
        public bool IsLoading { get; private set; }

        public void NotifySceneReady()
        {
            _sceneReady = true;
        }
        public IReadOnlyReactiveProperty<float> Progress => _progress;
        public IReadOnlyReactiveProperty<string> Stage => _stage;

        public SceneLoader(SceneLoadingConfig config, LoadingCurtainView curtain, CoroutineRunner runner)
        {
            _config = config;
            _curtain = curtain;
            _runner = runner;

            _curtain.Bind(_progress, _stage);
        }

        public void ReportProgress(string stage, float value)
        {
            _stage.Value = stage;
            _progress.Value = Mathf.Clamp01(value);
        }

        public void LoadGame(Action onLoaded = null)
        {
            Load(_config.GameSceneName, onLoaded);
        }

        public void Reload(Action onLoaded = null)
        {
            Load(SceneManager.GetActiveScene().name, onLoaded);
        }

        private void Load(string sceneName, Action onLoaded)
        {
            if (IsLoading)
                return;

            IsLoading = true;
            _runner.Run(LoadRoutine(sceneName, onLoaded));
        }

        private IEnumerator LoadRoutine(string sceneName, Action onLoaded)
        {
            _sceneReady = false;
            ReportProgress(string.Empty, 0f);

            yield return _curtain.FadeIn(_config.FadeInDuration);

            var startedAt = Time.unscaledTime;
            ReportProgress("Загрузка сцены", 0f);

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            operation.allowSceneActivation = true;

            while (!operation.isDone)
            {
                ReportProgress("Загрузка сцены", operation.progress * SceneLoadPortion);
                yield return null;
            }

            ReportProgress("Подготовка", SceneLoadPortion);

            while (!_sceneReady)
                yield return null;

            ReportProgress("Готово", 1f);

            var elapsed = Time.unscaledTime - startedAt;
            if (elapsed < _config.MinimumDisplayTime)
                yield return new WaitForSecondsRealtime(_config.MinimumDisplayTime - elapsed);

            yield return _curtain.FadeOut(_config.FadeOutDuration);

            IsLoading = false;
            onLoaded?.Invoke();
        }
    }
}
