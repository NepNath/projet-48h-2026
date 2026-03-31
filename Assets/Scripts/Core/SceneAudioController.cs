using UnityEngine;
using UnityEngine.SceneManagement;

// Central controller: when scenes load, trigger the appropriate music on SoundManager.
// Put one instance in the first scene (MainMenu). It uses DontDestroyOnLoad so it persists.
public class SceneAudioController : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // In case this object is added at runtime while a scene is already loaded
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[SceneAudioController] SoundManager.Instance not found on scene load: " + scene.name);
            return;
        }

        // Use known SceneFlow constants for special scenes
        if (scene.name == SceneFlow.MainMenuScene)
        {
            Debug.Log("[SceneAudioController] MainMenu loaded -> PlayMenuMusic");
            SoundManager.Instance.PlayMenuMusic();
            return;
        }

        if (scene.name == SceneFlow.QuestionScene)
        {
            Debug.Log("[SceneAudioController] Question loaded -> PlayQuestionMusic");
            SoundManager.Instance.PlayQuestionMusic();
            return;
        }

        if (scene.name == SceneFlow.GameOverScene)
        {
            // choose menu music or stop — here we stop
            Debug.Log("[SceneAudioController] GameOver loaded -> StopMusic");
            SoundManager.Instance.StopMusic();
            return;
        }

        // Otherwise treat as a mini-game scene
        Debug.Log("[SceneAudioController] Mini-game loaded (" + scene.name + ") -> PlayMiniGameMusic");
        SoundManager.Instance.PlayMiniGameMusic();
    }
}
