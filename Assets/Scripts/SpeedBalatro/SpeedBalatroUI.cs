using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

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
            // Subscribe to GameManager events
            ScoreUpdatedEvent.OnScoreUpdated += UpdateScoreUI;
            TimerUpdatedEvent.OnTimerUpdated += UpdateTimerUI;
            NewHandDealtEvent.OnNewHandDealt += DisplayNewHand;
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from GameManager events
            ScoreUpdatedEvent.OnScoreUpdated -= UpdateScoreUI;
            TimerUpdatedEvent.OnTimerUpdated -= UpdateTimerUI;
            NewHandDealtEvent.OnNewHandDealt -= DisplayNewHand;
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
            if (cardUI.IsSelected)
            {
                cardUI.SetSelected(false);
                cardUI.transform.position += new Vector3(0, -20f, 0);
                new CardDeselectedEvent(cardUI.GetCard()).Execute();
            }
            else
            {
                cardUI.SetSelected(true);
                cardUI.transform.position += new Vector3(0, 20f, 0);
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