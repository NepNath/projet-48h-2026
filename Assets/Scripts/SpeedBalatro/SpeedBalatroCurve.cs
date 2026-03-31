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

        public Coroutine newAnim;

        private struct CardTransform
        {
            public Vector2 anchoredPosition;
            public float rotation;
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

                if (!targetTransforms.TryGetValue(card, out CardTransform currentTarget) ||
                    currentTarget.anchoredPosition != targetPos ||
                    currentTarget.rotation != targetRot)
                {
                    targetTransforms[card] = new CardTransform
                    {
                        anchoredPosition = targetPos,
                        rotation = targetRot
                    };

                    if (activeAnimations.TryGetValue(card, out Coroutine existingAnim))
                    {
                        if (existingAnim != null)
                            StopCoroutine(existingAnim);
                        activeAnimations.Remove(card);
                    }

                    float delay = i * staggerDelay;
                    newAnim = StartCoroutine(AnimateCardToPosition(card, targetPos, targetRot, delay, i));
                    activeAnimations[card] = newAnim;
                }

                card.SetSiblingIndex(i);
            }

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
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            if (card == null || card.parent != transform)
                yield break;

            Vector2 startPos = card.anchoredPosition;
            float startRot = card.localRotation.eulerAngles.z;

            if (startRot > 180f) startRot -= 360f;

            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                if (card == null)
                    yield break;

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animationDuration);
                float easedT = easeCurve.Evaluate(t);

                Vector2 basePos = Vector2.Lerp(startPos, targetPos, easedT);

                float archT = Mathf.Sin(t * Mathf.PI);
                float archOffset = archT * archHeight;

                card.anchoredPosition = basePos + new Vector2(0f, archOffset);

                float currentRot = Mathf.LerpAngle(startRot, targetRot, easedT);
                card.localRotation = Quaternion.Euler(0f, 0f, currentRot);

                yield return null;
            }

            if (card != null)
            {
                card.anchoredPosition = targetPos;
                card.localRotation = Quaternion.Euler(0f, 0f, targetRot);
            }

            if (activeAnimations.ContainsKey(card))
                activeAnimations.Remove(card);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
