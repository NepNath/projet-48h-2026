using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public static string[] scenes = {};

    public static void loadRandomScene()
    {
        int randomIndex = Random.Range(0, scenes.Length);
        SceneManager.LoadScene(scenes[randomIndex]);
    }

    public static void quitGame()
    {
        Application.Quit();
    }
}
