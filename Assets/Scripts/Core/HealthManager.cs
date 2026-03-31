using UnityEngine;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    [SerializeField] private int maxLives = 3;
    [SerializeField] private TextMeshProUGUI livesUI;

    private int currentLives;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ResetHealth();
        UpdateUI();
    }

    public void ResetHealth()
    {
        currentLives = maxLives;
        UpdateUI();
    }

    public void LoseLife()
    {
        currentLives--;
        // prevent negative lives
        if (currentLives < 0) currentLives = 0;
        Debug.Log($"Lost a life! Current lives: {currentLives}");
        UpdateUI();

        if (currentLives <= 0)
        {
            Debug.Log("GameOver triggered!");
            GameOver();
        }
    }

    public int GetCurrentLives() => currentLives;
    public int GetMaxLives() => maxLives;

    private void UpdateUI()
    {
        if (livesUI != null)
            livesUI.text = $"Lives: {currentLives}/{maxLives}";
    }

    private void GameOver()
    {
        SceneFlow.ResetRun();
        TransitionManager.LoadScene(SceneFlow.GameOverScene);
    }
}
