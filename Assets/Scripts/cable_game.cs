    using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class cable_game : MonoBehaviour
{
    public float timeLimit = 8f;
    private float timeLeft;
    private bool gameActive = false;

    bool[] buttonsClicked = new bool[8];
    Button[] buttons = new Button[8];
    bool[] buttonsDone = new bool[4];
    GameObject victoryText;
    public Text timerText;

    void Start()
    {
        getButtonsAndCanvas();
        StartGame();
    }

    void StartGame()
    {
        int[] indices = {0,1,2,3,4,5,6,7};
        for (int i = 0; i < indices.Length; i++)
        {
            int rnd = Random.Range(i, indices.Length);
            int tmp = indices[i];
            indices[i] = indices[rnd];
            indices[rnd] = tmp;
        }

        for (int i = 0; i < buttonsClicked.Length; i++)
            buttonsClicked[i] = false;
        for (int i = 0; i < buttonsDone.Length; i++)
            buttonsDone[i] = false;
        victoryText.SetActive(false);
        timeLeft = timeLimit;
        gameActive = true;
        checkClickedButtons();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!gameActive) return;
        timeLeft -= Time.deltaTime;
        UpdateTimerUI();
        if (timeLeft <= 0f)
        {
            EndGame(false);
        }
    }

    void getButtonsAndCanvas()
    {
        buttons[0] = GameObject.Find("ButtonGrid/RedButton1").GetComponent<Button>();
        buttons[1] = GameObject.Find("ButtonGrid/RedButton2").GetComponent<Button>();
        buttons[2] = GameObject.Find("ButtonGrid/BlueButton1").GetComponent<Button>();
        buttons[3] = GameObject.Find("ButtonGrid/BlueButton2").GetComponent<Button>();
        buttons[4] = GameObject.Find("ButtonGrid/GreenButton1").GetComponent<Button>();
        buttons[5] = GameObject.Find("ButtonGrid/GreenButton2").GetComponent<Button>();
        buttons[6] = GameObject.Find("ButtonGrid/YellowButton1").GetComponent<Button>();
        buttons[7] = GameObject.Find("ButtonGrid/YellowButton2").GetComponent<Button>();
        victoryText = GameObject.Find("StateGrid/VictoryText");
        victoryText.SetActive(false);
        timerText = GameObject.Find("StateGrid/TimerText")?.GetComponent<Text>();
    }

    public void checkClickedButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].onClick.RemoveAllListeners();
            buttons[index].onClick.AddListener(() => onButtonClicked(index));
        }
    }

    void onButtonClicked(int index)
    {
        int pairIndex = index / 2;
        int firstButton = pairIndex * 2;
        int secondButton = pairIndex * 2 + 1;
        for (int i = 0; i < buttonsClicked.Length; i++)
        {
            if (i != firstButton && i != secondButton)
                buttonsClicked[i] = false;
        }
        buttonsClicked[index] = true;
        checkButtons();
    }

    void checkButtons()
    {
        for (int i = 0; i < buttonsClicked.Length; i += 2)
        {
            int index = i;
            if (buttonsClicked[index] && buttonsClicked[index + 1] && !buttonsDone[index / 2])
            {
                buttonsDone[index/2] = true;
                buttons[index].gameObject.SetActive(false);
                buttons[index + 1].gameObject.SetActive(false);
                buttons[index].interactable = false;
                buttons[index + 1].interactable = false;
                checkVictory();
            }
        }
    }

    void checkVictory()
    {
        if (buttonsDone[0] && buttonsDone[1] && buttonsDone[2] && buttonsDone[3])
        {
            EndGame(true);
        }
    }

    void EndGame(bool win)
    {
        gameActive = false;
        victoryText.SetActive(win);
        StartCoroutine(LoadNextScene(win));
    }

    IEnumerator LoadNextScene(bool win)
    {
        yield return new WaitForSeconds(0.5f);
        TransitionManager.LoadScene(SceneFlow.CompleteMiniGame(win));
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }
}