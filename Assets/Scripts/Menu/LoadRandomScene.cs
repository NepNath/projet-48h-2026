using UnityEngine;
using UnityEngine.SceneManagement;

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
        TransitionManager.LoadScene(menuScene);
    }

    public void loadSettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }
}

public class ButtonManager : LoadRandomScene
{
}
