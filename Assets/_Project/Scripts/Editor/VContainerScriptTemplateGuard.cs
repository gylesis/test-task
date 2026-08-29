using UnityEditor;
using VContainer.Unity;

namespace Project.Editor
{
    [InitializeOnLoad]
    public static class VContainerScriptTemplateGuard
    {
        static VContainerScriptTemplateGuard()
        {
            VContainerSettings.LoadInstanceFromPreloadAssets();
        }
    }
}
