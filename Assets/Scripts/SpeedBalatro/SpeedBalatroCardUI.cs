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

            foreach (var result in raycastResults)
            {
                if (result.gameObject == gameObject)
                {
                    onCardClicked?.Invoke(this);
                    break;
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

            if (cardImage != null)
            {
                cardImage.color = isSelected ? Color.yellow : cardData.cardColor;
            }

            // Visual feedback: move card up when selected, back to original when deselected
            if (isSelected)
            {
                transform.position += new Vector3(0, 20, 0);
            }
            else
            {
                transform.position -= new Vector3(0, 20, 0);
            } 
            
        }

        public Card GetCard()
        {
            return cardData;
        }
    }
}
