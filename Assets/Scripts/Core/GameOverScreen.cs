using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (gameOverText != null)
            gameOverText.text = "rip bozo";

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetry);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);
    }

    private void OnRetry()
    {
        if (HealthManager.Instance == null)
        {
            var go = new GameObject("HealthManager");
            go.AddComponent<HealthManager>();
        }
        HealthManager.Instance?.ResetHealth();
        TransitionManager.LoadScene(SceneFlow.MainMenuScene);
    }

    private void OnQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
