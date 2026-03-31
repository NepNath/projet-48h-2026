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
        if (currentLives < 0) currentLives = 0;
        UpdateUI();

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public int GetCurrentLives() => currentLives;
    public int GetMaxLives() => maxLives;

    public void GainLife(int amount = 1)
    {
        if (amount <= 0) return;
        currentLives += amount;
        if (currentLives > maxLives) currentLives = maxLives;
        UpdateUI();
    }

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
