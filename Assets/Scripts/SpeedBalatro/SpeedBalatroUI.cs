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
        [SerializeField] private Transform handContainer;
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private Button submitHandButton;
        [SerializeField] private float cardSpacing = 120f; // horizontal spacing in pixels between cards

        [Header("Game State UI")]
        [SerializeField] private GameObject gameOverMenu;
        [SerializeField] private GameObject victoryMenu;
        [SerializeField] private Button restartButton;

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
            // Subscribe to GameManager events
            ScoreUpdatedEvent.OnScoreUpdated += UpdateScoreUI;
            TimerUpdatedEvent.OnTimerUpdated += UpdateTimerUI;
            NewHandDealtEvent.OnNewHandDealt += DisplayNewHand;
            GameStateChangedEvent.OnGameStateChanged += UpdateGameState;
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from GameManager events
            ScoreUpdatedEvent.OnScoreUpdated -= UpdateScoreUI;
            TimerUpdatedEvent.OnTimerUpdated -= UpdateTimerUI;
            NewHandDealtEvent.OnNewHandDealt -= DisplayNewHand;
            GameStateChangedEvent.OnGameStateChanged -= UpdateGameState;
        }

        private void InitializeUI()
        {
            if (gameManager != null)
            {
                UpdateTargetScoreUI(gameManager.GetTargetScore());
            }

            // Hide menus initially
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

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(() => 
                    new GameRestartEvent().Execute());
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
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";

                // Change color when time is running low
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

        private void UpdateGameState(string newState)
        {
            if (gameStateText != null)
            {
                gameStateText.text = newState;
            }

            // Handle game state changes
            switch (newState)
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
                case "Get Ready!":
                    HideAllMenus();
                    break;
            }
        }

        private void DisplayNewHand(Card[] newHand)
        {
            // Clear existing hand UI
            ClearHandUI();

            // If a layout group is on the parent, disable it to use manual placement
            if (handContainer != null)
            {
                var layoutGroup = handContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                if (layoutGroup != null)
                    layoutGroup.enabled = false;
            }

            // Create new card UI elements with manual spacing
            float startX = (newHand.Length - 1) * -0.5f * cardSpacing;
            for (int i = 0; i < newHand.Length; i++)
            {
                GameObject cardUIObject = Instantiate(cardUIPrefab, handContainer);
                if (cardUIObject != null)
                {
                    // Keep uniform scale and set anchored position
                    var cardRect = cardUIObject.GetComponent<RectTransform>();
                    if (cardRect != null)
                    {
                        cardRect.localScale = Vector3.one;
                        cardRect.anchoredPosition = new Vector2(startX + i * cardSpacing, 0f);
                    }

                    CardUI cardUI = cardUIObject.GetComponent<CardUI>();
                    if (cardUI != null)
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
            if (cardUI.IsSelected)
            {
                // Deselect card
                cardUI.SetSelected(false);
                new CardDeselectedEvent(cardUI.GetCard()).Execute();
            }
            else
            {
                // Select card
                cardUI.SetSelected(true);
                new CardSelectedEvent(cardUI.GetCard()).Execute();
            }

            UpdateSubmitButtonState();
        }

        private void UpdateSubmitButtonState()
        {
            if (submitHandButton != null)
            {
                int selectedCount = GetSelectedCardCount();
                submitHandButton.interactable = selectedCount > 0;
                
                // Update button text
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
                new HandSubmittedEvent(selectedCards.ToArray()).Execute();
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