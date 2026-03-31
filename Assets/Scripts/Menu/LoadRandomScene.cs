using UnityEngine;

public class LoadRandomScene : MonoBehaviour
{
    [SerializeField] private string menuScene = SceneFlow.MainMenuScene;

    public void loadScene()
    {
        loadRandomScene();
    }

    public void loadRandomScene()
    {
        TransitionManager.LoadScene(SceneFlow.StartRun());
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void loadMenuScene()
    {
        SceneFlow.ResetRun();
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayMenuMusic();
        TransitionManager.LoadScene(menuScene);
    }
}

public class ButtonManager : LoadRandomScene
{
}
