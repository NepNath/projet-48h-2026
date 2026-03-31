using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace SpeedBalatro
{
    public class SpeedBalatroUI : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI targetScoreText;
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI handTypeLabel;

        [Header("Hand Display")]
        [SerializeField] private GameObject handContainer;
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private Button submitButton;
        [SerializeField] private CurvedCardLayout curvedCardLayout;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverMessageText;

        [Header("References")]
        [SerializeField] private GameManager gameManager;

        private List<CardUI> currentHandUI = new();

        private void Start()
        {
            SubscribeToEvents();
            InitializeUI();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            SpeedBalatroEvents.OnTimerUpdated += UpdateTimerUI;
            SpeedBalatroEvents.OnNewHandDealt += DisplayNewHand;
            SpeedBalatroEvents.OnHandScoreInfoUpdated += UpdateHandDisplay;
            SpeedBalatroEvents.OnTargetScoreChanged += UpdateTargetScoreUI;
            SpeedBalatroEvents.OnGameWon += HandleGameWon;
            SpeedBalatroEvents.OnGameLost += HandleGameLost;
        }

        private void UnsubscribeFromEvents()
        {
            SpeedBalatroEvents.OnTimerUpdated -= UpdateTimerUI;
            SpeedBalatroEvents.OnNewHandDealt -= DisplayNewHand;
            SpeedBalatroEvents.OnHandScoreInfoUpdated -= UpdateHandDisplay;
            SpeedBalatroEvents.OnTargetScoreChanged -= UpdateTargetScoreUI;
            SpeedBalatroEvents.OnGameWon -= HandleGameWon;
            SpeedBalatroEvents.OnGameLost -= HandleGameLost;
        }


        private void InitializeUI()
        {
            if (gameManager != null)
            {
                UpdateTargetScoreUI(gameManager.GetTargetScore());
            }

            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (handTypeLabel != null) handTypeLabel.text = "";
            if (currentScoreText != null) currentScoreText.text = "";
            if (submitButton != null)
            {
                submitButton.interactable = false;
                submitButton.onClick.AddListener(SubmitSelectedCards);
            }

        }

        private void UpdateTimerUI(float timeRemaining)
        {
            if (timeText != null)
            {
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                int millis = Mathf.FloorToInt(timeRemaining * 100 % 100);
                timeText.text = $"{seconds:00}:{millis:00}";

                if (timeRemaining <= 10f)
                {
                    timeText.color = Color.red;
                }
                else
                {
                    timeText.color = Color.white;
                }
            }
        }

        private void UpdateTargetScoreUI(float targetScore)
        {
            if (targetScoreText != null)
            {
                targetScoreText.text = $"Target: {targetScore:N0}";
            }
        }
        private void UpdateCurrentScoreUI(float currentScore)
        {
            if (currentScoreText != null)
            {
                currentScoreText.text = $"{currentScore:N0}";
            }
        }

        private void UpdateHandDisplay(GameManager.HandScoreInfo info)
        {
            if (info.totalScore == 0)
            {
                handTypeLabel.text = "";
                UpdateCurrentScoreUI(info.totalScore);
                
            }
            else
            {
                handTypeLabel.text = info.handType.ToString();
                UpdateCurrentScoreUI(info.totalScore);
            }
        }

        private void DisplayNewHand(Card[] newHand)
        {
            ClearHandUI();

            for (int i = 0; i < newHand.Length; i++)
            {
                GameObject cardUIObject = Instantiate(cardUIPrefab, handContainer.transform);
                if (cardUIObject != null)
                {
                    if (cardUIObject.TryGetComponent<CardUI>(out var cardUI))
                    {
                        cardUI.SetupCard(newHand[i], OnCardClicked);
                        currentHandUI.Add(cardUI);
                    }
                }
            }

            if (curvedCardLayout != null)
            {
                curvedCardLayout.ArrangeCards();
            }

            UpdateSubmitButtonState();
        }

        private void OnCardClicked(CardUI cardUI)
        {
            bool willBeSelected = !cardUI.IsSelected;
            cardUI.SetSelected(willBeSelected);

            if (willBeSelected)
            {
                SpeedBalatroEvents.RaiseCardSelected(cardUI.GetCard());
            }
            else
            {
                SpeedBalatroEvents.RaiseCardDeselected(cardUI.GetCard());
            }

            UpdateSubmitButtonState();
        }

        private void UpdateSubmitButtonState()
        {
            if (submitButton == null) return;

            int count = 0;
            foreach (CardUI cardUI in currentHandUI)
                if (cardUI.IsSelected) count++;

            submitButton.interactable = count > 0;

            var buttonText = submitButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = count > 0 ? "Submit Hand" : "Select Cards";
        }

        public void SubmitSelectedCards()
        {
            var selected = currentHandUI.Where(c => c.IsSelected).Select(c => c.GetCard()).ToArray();
            if (selected.Length > 0)
                SpeedBalatroEvents.RaiseHandSubmitted(selected);
        }
        private void ClearHandUI()
        {
            foreach (CardUI cardUI in currentHandUI)
            {
                if (cardUI != null && cardUI.gameObject != null)
                {
                    Destroy(cardUI.gameObject);
                }
            }
            currentHandUI.Clear();
        }

        private void HandleGameWon(string message)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (gameOverMessageText != null)
            {
                gameOverMessageText.text = message;
                gameOverMessageText.color = Color.green;
            }
            StartCoroutine(LoadNextScene(true));

            DisableGameControls();
        }

        private void HandleGameLost(string message)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (gameOverMessageText != null)
            {
                gameOverMessageText.text = message;
                gameOverMessageText.color = Color.red;
            }
        StartCoroutine(LoadNextScene(false));

            DisableGameControls();
        }

        private void DisableGameControls()
        {
            if (submitButton != null)
                submitButton.interactable = false;

            foreach (var cardUI in currentHandUI)
            {
                if (cardUI != null)
                {
                    Button cardButton = cardUI.GetComponent<Button>();
                    if (cardButton != null)
                        cardButton.interactable = false;
                }
            }
        }

        IEnumerator LoadNextScene(bool win)
        {
            yield return new WaitForSeconds(0.5f);
            TransitionManager.LoadScene(SceneFlow.CompleteMiniGame(win));
        }

    }
}
