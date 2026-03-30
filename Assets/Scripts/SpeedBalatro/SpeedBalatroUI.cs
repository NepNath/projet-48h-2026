using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SpeedBalatro
{
    public class SpeedBalatroUI : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI targetScoreText;
        [SerializeField] private TextMeshProUGUI gameStateText;

        [Header("Hand Display")]
        [SerializeField] private GameObject handContainer;
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private Button submitHandButton;

        [Header("Game State UI")]
        [SerializeField] private GameObject gameOverMenu;
        [SerializeField] private GameObject victoryMenu;

        [Header("References")]
        [SerializeField] private GameManager gameManager;

        private List<CardUI> currentHandUI = new();

        private void Start()
        {
            SubscribeToEvents();
            InitializeUI();
            SetupButtons();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            SpeedBalatroEvents.OnScoreUpdated += UpdateScoreUI;
            SpeedBalatroEvents.OnTimerUpdated += UpdateTimerUI;
            SpeedBalatroEvents.OnNewHandDealt += DisplayNewHand;
            SpeedBalatroEvents.OnGameStateChanged += HandleGameStateChanged;
        }

        private void UnsubscribeFromEvents()
        {
            SpeedBalatroEvents.OnScoreUpdated -= UpdateScoreUI;
            SpeedBalatroEvents.OnTimerUpdated -= UpdateTimerUI;
            SpeedBalatroEvents.OnNewHandDealt -= DisplayNewHand;
            SpeedBalatroEvents.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void InitializeUI()
        {
            if (gameManager != null)
            {
                UpdateTargetScoreUI(gameManager.GetTargetScore());
            }

            if (gameOverMenu != null) gameOverMenu.SetActive(false);
            if (victoryMenu != null) victoryMenu.SetActive(false);
        }

        private void SetupButtons()
        {
            if (submitHandButton != null)
            {
                submitHandButton.onClick.AddListener(SubmitSelectedCards);
                submitHandButton.interactable = false;
            }
        }

        private void UpdateScoreUI(float newScore)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {newScore:N0}";
            }
        }

        private void UpdateTimerUI(float timeRemaining)
        {
            if (timeText != null)
            {
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                int millis = Mathf.FloorToInt(timeRemaining * 100 % 100);
                timeText.text = $"Time: {seconds:00}:{millis:00}";

                if (timeRemaining <= 10f)
                {
                    timeText.color = Color.red;
                }
                else if (timeRemaining <= 30f)
                {
                    timeText.color = Color.yellow;
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

        private void HandleGameStateChanged(string state)
        {
            if (gameStateText != null)
            {
                gameStateText.text = state;
            }

            switch (state)
            {
                case "Victory!":
                    ShowVictoryMenu();
                    break;
                case "Time's Up!":
                    ShowGameOverMenu();
                    break;
                case "Playing":
                    HideAllMenus();
                    break;
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
            if (submitHandButton != null)
            {
                int selectedCount = GetSelectedCardCount();
                submitHandButton.interactable = selectedCount > 0;

                TextMeshProUGUI buttonText = submitHandButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = selectedCount > 0 ?
                        $"Submit Hand ({selectedCount})" : "Select Cards";
                }
            }
        }

        private int GetSelectedCardCount()
        {
            int count = 0;
            foreach (CardUI cardUI in currentHandUI)
            {
                if (cardUI.IsSelected) count++;
            }
            return count;
        }

        private void SubmitSelectedCards()
        {
            List<Card> selectedCards = new();
            foreach (CardUI cardUI in currentHandUI)
            {
                if (cardUI.IsSelected)
                {
                    selectedCards.Add(cardUI.GetCard());
                }
            }

            if (selectedCards.Count > 0)
            {
                SpeedBalatroEvents.RaiseHandSubmitted(selectedCards.ToArray());
            }
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

        private void ShowVictoryMenu()
        {
            if (victoryMenu != null)
            {
                victoryMenu.SetActive(true);
            }
        }

        private void ShowGameOverMenu()
        {
            if (gameOverMenu != null)
            {
                gameOverMenu.SetActive(true);
            }
        }

        private void HideAllMenus()
        {
            if (gameOverMenu != null) gameOverMenu.SetActive(false);
            if (victoryMenu != null) victoryMenu.SetActive(false);
        }
    }
}
