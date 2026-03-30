using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.InputSystem;

namespace SpeedBalatro
{
    public class CardUI : MonoBehaviour, IPointerClickHandler
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
            onCardClicked?.Invoke(this);
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                CheckCardClick();
            }
        }

        // WORKAROUND: The project's InputSystem is configured with "Both" input modes
        // (activeInputHandler: 1 in ProjectSettings). This causes the EventSystem's
        // IPointerClickHandler to not receive click events properly, even though
        // raycasts work correctly. Using InputSystem's Mouse API for click detection
        // and manually invoking the callback bypasses this issue.
        // 
        // Alternative: Set activeInputHandler to 2 (InputSystem Only) in ProjectSettings,
        // but this may break other parts of the game that rely on legacy Input.
        private void CheckCardClick()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
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
