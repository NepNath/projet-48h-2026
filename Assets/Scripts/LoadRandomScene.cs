using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadRandomScene : MonoBehaviour
{
    public string[] scenes = {};

    public void loadScene()
    {
        int randomIndex = Random.Range(0, scenes.Length);
        SceneManager.LoadScene(scenes[randomIndex]);
    }
}
