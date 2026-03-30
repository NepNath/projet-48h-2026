using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpeedBalatro
{
    public class CurvedCardLayout : MonoBehaviour
    {
        [Header("Layout Settings")]
        [SerializeField] private float spacing = 120f;
        [SerializeField] private float curveHeight = 60f;
        [SerializeField] private float maxRotation = 15f;

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.4f;
        [SerializeField] private float staggerDelay = 0.05f;
        [SerializeField] private float archHeight = 80f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private Dictionary<RectTransform, Coroutine> activeAnimations = new Dictionary<RectTransform, Coroutine>();
        private Dictionary<RectTransform, CardTransform> targetTransforms = new Dictionary<RectTransform, CardTransform>();

        private struct CardTransform
        {
            public Vector2 anchoredPosition;
            public float rotation;
        }

        private void LateUpdate()
        {
            ArrangeCards();
        }

        public void ArrangeCards()
        {
            int count = transform.childCount;
            if (count == 0) return;

            float totalWidth = (count - 1) * spacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                RectTransform card = transform.GetChild(i) as RectTransform;
                if (card == null) continue;

                // Skip repositioning if card is selected (being interacted with)
                CardUI cardUI = card.GetComponent<CardUI>();
                if (cardUI != null && cardUI.IsSelected)
                    continue;

                float x = startX + i * spacing;

                float normalized =
                    count == 1 ? 0f : (i / (float)(count - 1)) * 2f - 1f;

                float y = -Mathf.Pow(normalized, 2f) * curveHeight + curveHeight;
                float rotation = -normalized * maxRotation;

                Vector2 targetPos = new Vector2(x, y);
                float targetRot = rotation;

                // Check if position has changed
                if (!targetTransforms.TryGetValue(card, out CardTransform currentTarget) ||
                    currentTarget.anchoredPosition != targetPos ||
                    currentTarget.rotation != targetRot)
                {
                    // Store new target
                    targetTransforms[card] = new CardTransform
                    {
                        anchoredPosition = targetPos,
                        rotation = targetRot
                    };

                    // Stop existing animation if any
                    if (activeAnimations.TryGetValue(card, out Coroutine existingAnim))
                    {
                        if (existingAnim != null)
                            StopCoroutine(existingAnim);
                        activeAnimations.Remove(card);
                    }

                    // Start new animation with stagger delay
                    float delay = i * staggerDelay;
                    Coroutine newAnim = StartCoroutine(AnimateCardToPosition(card, targetPos, targetRot, delay, i));
                    activeAnimations[card] = newAnim;
                }

                card.SetSiblingIndex(i);
            }

            // Clean up dictionary entries for removed cards
            List<RectTransform> toRemove = new List<RectTransform>();
            foreach (var key in targetTransforms.Keys)
            {
                if (key == null || key.parent != transform)
                    toRemove.Add(key);
            }
            foreach (var key in toRemove)
            {
                targetTransforms.Remove(key);
                activeAnimations.Remove(key);
            }
        }

        private IEnumerator AnimateCardToPosition(RectTransform card, Vector2 targetPos, float targetRot, float delay, int siblingIndex)
        {
            // Wait for stagger delay
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            // Check if card still exists and is still child of this transform
            if (card == null || card.parent != transform)
                yield break;

            Vector2 startPos = card.anchoredPosition;
            float startRot = card.localRotation.eulerAngles.z;

            // Normalize rotation to -180 to 180 range for smooth interpolation
            if (startRot > 180f) startRot -= 360f;

            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                // Check if card still exists
                if (card == null)
                    yield break;

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animationDuration);
                float easedT = easeCurve.Evaluate(t);

                // Linear interpolation for base position
                Vector2 basePos = Vector2.Lerp(startPos, targetPos, easedT);

                // Add arch effect (parabolic arc)
                float archT = Mathf.Sin(t * Mathf.PI); // 0 -> 1 -> 0
                float archOffset = archT * archHeight;

                card.anchoredPosition = basePos + new Vector2(0f, archOffset);

                // Smooth rotation interpolation
                float currentRot = Mathf.LerpAngle(startRot, targetRot, easedT);
                card.localRotation = Quaternion.Euler(0f, 0f, currentRot);

                yield return null;
            }

            // Ensure final position is exact
            if (card != null)
            {
                card.anchoredPosition = targetPos;
                card.localRotation = Quaternion.Euler(0f, 0f, targetRot);
            }

            // Remove from active animations
            if (activeAnimations.ContainsKey(card))
                activeAnimations.Remove(card);
        }

        private void OnDestroy()
        {
            // Stop all coroutines when destroyed
            StopAllCoroutines();
        }
    }
}
