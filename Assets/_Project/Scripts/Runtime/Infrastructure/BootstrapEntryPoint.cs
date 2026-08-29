using Project.UI;
using VContainer.Unity;

namespace Project.Infrastructure
{
    public sealed class BootstrapEntryPoint : IStartable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtainView _curtain;

        public BootstrapEntryPoint(SceneLoader sceneLoader, LoadingCurtainView curtain)
        {
            _sceneLoader = sceneLoader;
            _curtain = curtain;
        }

        public void Start()
        {
            _curtain.ShowInstant();
            _sceneLoader.LoadGame();
        }
    }
}
