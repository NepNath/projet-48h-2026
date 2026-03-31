using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SpeedBalatro
{
    public class SpeedBalatroUI : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI targetScoreText;
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI handTypeLabel;
        [SerializeField] private TextMeshProUGUI chipsLabel;
        [SerializeField] private TextMeshProUGUI multLabel;

        [Header("Hand Display")]
        [SerializeField] private GameObject handContainer;
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private Button submitButton;
        [SerializeField] private CurvedCardLayout curvedCardLayout;

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
            SpeedBalatroEvents.OnTimerUpdated += UpdateTimerUI;
            SpeedBalatroEvents.OnNewHandDealt += DisplayNewHand;
            SpeedBalatroEvents.OnScoreUpdated += UpdateCurrentScoreUI;
            SpeedBalatroEvents.OnHandScoreInfoUpdated += UpdateHandDisplay;
        }

        private void UnsubscribeFromEvents()
        {
            SpeedBalatroEvents.OnTimerUpdated -= UpdateTimerUI;
            SpeedBalatroEvents.OnNewHandDealt -= DisplayNewHand;
            SpeedBalatroEvents.OnScoreUpdated -= UpdateCurrentScoreUI;
            SpeedBalatroEvents.OnHandScoreInfoUpdated -= UpdateHandDisplay;
        }


        private void InitializeUI()
        {
            if (gameManager != null)
            {
                UpdateTargetScoreUI(gameManager.GetTargetScore());
            }

            if (handTypeLabel != null) handTypeLabel.text = "";
            if (chipsLabel != null) chipsLabel.text = "0";
            if (multLabel != null) multLabel.text = "x0";

            if (gameOverMenu != null) gameOverMenu.SetActive(false);
            if (victoryMenu != null) victoryMenu.SetActive(false);
        }

        private void SetupButtons()
        {
            if (submitButton != null)
            {
                submitButton.onClick.AddListener(SubmitSelectedCards);
                submitButton.interactable = false;
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
                chipsLabel.text = "0";
                multLabel.text = "x0";
            }
            else
            {
                handTypeLabel.text = info.handType.ToString();
                chipsLabel.text = info.chips.ToString();
                multLabel.text = $"x{info.mult}";
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
            if (submitButton != null)
            {
                int selectedCount = GetSelectedCardCount();
                submitButton.interactable = selectedCount > 0;

                TextMeshProUGUI buttonText = submitButton.GetComponentInChildren<TextMeshProUGUI>();
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
