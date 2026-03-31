#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[InitializeOnLoad]
public static class BuildSettingsHelper
{
    static BuildSettingsHelper()
    {
        // ensure important scenes are present in Build Settings so SceneManager can load them
        AddSceneIfMissing("Assets/Scenes/Perdu.unity");
        AddSceneIfMissing("Assets/Scenes/Question.unity");
        AddSceneIfMissing("Assets/Scenes/MainMenu.unity");
        AddSceneIfMissing("Assets/Scenes/KeyCard.unity");
        AddSceneIfMissing("Assets/Scenes/DigitCode.unity");
    }

    static void AddSceneIfMissing(string path)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (!scenes.Any(s => s.path == path))
        {
            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
#endif
