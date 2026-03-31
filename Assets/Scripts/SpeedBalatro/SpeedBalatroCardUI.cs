using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SpeedBalatro
{
    public class CardUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image cardImage;
        [SerializeField] private GameObject selectionHighlight;
        [SerializeField] private Button cardButton;

        private Card cardData;
        private bool isSelected;
        private System.Action<CardUI> onCardClicked;
        private Vector3 originalPosition;

        public bool IsSelected => isSelected;

        public void SetupCard(Card card, System.Action<CardUI> clickCallback)
        {
            cardData = card;
            onCardClicked = clickCallback;
            UpdateCardDisplay();
            
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(OnCardButtonClicked);
            }
        }

        private void OnDestroy()
        {
            if (cardButton != null)
            {
                cardButton.onClick.RemoveListener(OnCardButtonClicked);
            }
        }

        private void UpdateCardDisplay()
        {
            if (cardData == null) return;

            if (cardImage != null && cardData.cardSprite != null)
            {
                cardImage.sprite = cardData.cardSprite;
                cardImage.color = cardData.cardColor;
            }
        }

        private void OnCardButtonClicked()
        {
            onCardClicked?.Invoke(this);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }

            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (isSelected)
                {
                    rectTransform.anchoredPosition += new Vector2(0, 50);
                }
                else
                {
                    rectTransform.anchoredPosition += new Vector2(0, -50);
                }
            }
        }

        public Card GetCard()
        {
            return cardData;
        }
    }
}
