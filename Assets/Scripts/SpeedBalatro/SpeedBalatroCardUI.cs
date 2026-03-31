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
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onCardClicked?.Invoke(this);
        }

        private void Update()
        {
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                CheckCardClick();
            }
        }

        private void CheckCardClick()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePosition
            };

            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                CardUI cardUI = result.gameObject.GetComponent<CardUI>();
                if (cardUI != null)
                {
                    if (cardUI == this)
                    {
                        onCardClicked?.Invoke(this);
                    }
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
