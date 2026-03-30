using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

namespace SpeedBalatro
{
    public class CardUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI Components")]
        [SerializeField] private Image cardImage;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private GameObject selectionHighlight;

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
        }

        private void UpdateCardDisplay()
        {
            if (cardData == null) return;

            if (cardImage != null && cardData.cardSprite != null)
            {
                cardImage.sprite = cardData.cardSprite;
                cardImage.color = cardData.cardColor;
            }

            if (cardNameText != null)
            {
                cardNameText.text = cardData.cardName;
            }

            if (valueText != null)
            {
                valueText.text = cardData.GetValue().ToString("F0");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // Fallback - also respond to EventSystem clicks if they work
            onCardClicked?.Invoke(this);
        }

        private void Update()
        {
            // Manual click detection using Input System (bypasses EventSystem issues)
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                CheckCardClick();
            }
        }

        private void CheckCardClick()
        {
            // Use Input System to get mouse position
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePosition
            };

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            // Find the first (top-most) card in the raycast results
            foreach (var result in raycastResults)
            {
                CardUI cardUI = result.gameObject.GetComponent<CardUI>();
                if (cardUI != null)
                {
                    // Only trigger if this is the top-most card AND it's this card
                    if (cardUI == this)
                    {
                        onCardClicked?.Invoke(this);
                    }
                    // Always break after finding the first card to ensure only top-most is selected
                    return;
                }
            }
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(isSelected);
            }

            // Visual feedback: move card up when selected, back to original when deselected
            // Use anchoredPosition to stay consistent with the curve layout
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
