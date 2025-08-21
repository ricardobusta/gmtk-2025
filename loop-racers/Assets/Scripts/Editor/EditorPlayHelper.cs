using UnityEditor;
using UnityEditor.SceneManagement;

namespace Busta.LoopRacers.Editor
{
    [InitializeOnLoad]
    public static class EditorPlayHelper
    {
        static EditorPlayHelper()
        {
            var scenePath = EditorBuildSettings.scenes[0].path;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        }
    }
}
