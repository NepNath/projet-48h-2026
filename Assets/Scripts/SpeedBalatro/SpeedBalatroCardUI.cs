using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SpeedBalatro
{
    public class CardUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Button cardButton;
        [SerializeField] private GameObject selectionHighlight;

        private Card cardData;
        private bool isSelected;
        private System.Action<CardUI> onCardClicked;

        public bool IsSelected => isSelected;

        public void SetupCard(Card card, System.Action<CardUI> clickCallback)
        {
            cardData = card;
            onCardClicked = clickCallback;

            UpdateCardDisplay();
            SetupButton();
        }

        private void UpdateCardDisplay()
        {
            if (cardData == null) return;

            // Set card image
            if (cardImage != null && cardData.cardSprite != null)
            {
                cardImage.sprite = cardData.cardSprite;
                cardImage.color = cardData.cardColor;
            }

            // Set card name
            if (cardNameText != null)
            {
                cardNameText.text = cardData.cardName;
            }

            // Set card value
            if (valueText != null)
            {
                valueText.text = cardData.GetValue().ToString("F0");
            }
        }

        private void SetupButton()
        {
            if (cardButton != null)
            {
                cardButton.onClick.RemoveAllListeners();
                cardButton.onClick.AddListener(() => onCardClicked?.Invoke(this));
            }
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }

            // Optional: Change card appearance when selected
            if (cardImage != null)
            {
                cardImage.color = isSelected ? 
                    Color.yellow : cardData.cardColor;
            }
        }

        public Card GetCard()
        {
            return cardData;
        }
    }
}