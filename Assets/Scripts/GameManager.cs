using UnityEngine;
using TMPro;

public class DigitCodeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI codeToTypeText;
    public TextMeshProUGUI playerInputText; 
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;

    [Header("Settings")]
    public float timeLimit = 5f;

    private string secretCode = "";
    private string playerInput = "";
    private float timeLeft;
    private bool gameActive = false;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {

        secretCode = Random.Range(1000, 9999).ToString();
        playerInput = "";
        timeLeft = timeLimit;
        gameActive = true;

        resultText.text = "";
        UpdateUI();
    }

    void Update()
    {
        if (!gameActive) return;

        timeLeft -= Time.deltaTime;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (timeLeft <= 0f)
        {
            EndGame(false);
        }
    }

    public void PressDigit(int digit)
    {
        if (!gameActive)return;
        if (playerInput.Length >= 4) return;

        playerInput += digit.ToString();
        UpdateUI();
        if (playerInput.Length == 4)
        {
            CheckCode();
        }
    }

    public void PressDelete()
    {
        if (!gameActive) return;
        if (playerInput.Length == 0) return;

        playerInput = playerInput.Substring(0, playerInput.Length - 1);
        UpdateUI();
    }

    void CheckCode()
    {
        if (!gameActive) return;
        if (playerInput == secretCode)
        {
            EndGame(true);
        }
        else
        {
            resultText.text = "NOPE";
            playerInput = "";
            UpdateUI();
        }
    }

    void EndGame(bool win)
    {
        gameActive = false;
        resultText.text = win ? "ACCESS GRANTED" : "ACCESS DENIED";
        StartCoroutine(LoadNextScene(win));
    }

    System.Collections.IEnumerator LoadNextScene(bool win)
    {
        yield return new WaitForSeconds(0.5f);
        TransitionManager.LoadScene(SceneFlow.CompleteMiniGame());
    }

    void UpdateUI()
    {
        codeToTypeText.text = secretCode;

        string display = "";
        for (int i = 0; i < 4; i++)
        {
            display += (i < playerInput.Length) ? playerInput[i] + " " : "_ ";
        }

        playerInputText.text = display.Trim();
    }
}
