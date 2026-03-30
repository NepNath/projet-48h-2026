using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SpeedBalatro;
using UnityEngine.Rendering;
using System.Collections;

namespace SpeedBalatro
{

    public class GameManager : MonoBehaviour
    {

        [Header("Card Settings")]
        public List<Card> availableCards;

        [Header("Round Settings")]
        public float targetScore;
        public float roundTimeLimit;

        [Header("Current Game State")]
        public List<Card> currentHand = new();
        public List<Card> selectedCards = new();


        private float currentScore;
        private float currentTime;
        private bool gameActive;
        private readonly HashSet<string> dealtHandKeys = new();


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeGame();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            CardSelectedEvent.OnCardSelected += HandleCardSelected;
            CardDeselectedEvent.OnCardDeselected += HandleCardDeselected;
            TimerExpiredEvent.OnTimerExpired += HandleTimerExpired;
            GameRestartEvent.OnGameRestart += HandleGameRestart;
        }

        private void Destroy()
        {
            CardSelectedEvent.OnCardSelected -= HandleCardSelected;
            CardDeselectedEvent.OnCardDeselected -= HandleCardDeselected;
            TimerExpiredEvent.OnTimerExpired -= HandleTimerExpired;
            GameRestartEvent.OnGameRestart -= HandleGameRestart;
        }

        private void UpdateTimer()
        {
            currentTime -= Time.deltaTime;
            new TimerUpdatedEvent(currentTime).Execute();
            if (currentTime <= 0)
            {
                new TimerExpiredEvent().Execute();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (gameActive)
            {
                UpdateTimer();
            }
        }

        void InitializeGame()
        {

            currentScore = 0;
            currentTime = roundTimeLimit;
            gameActive = true;

            new ScoreUpdatedEvent(currentScore).Execute();
            new GameStateChangedEvent("Get Ready!").Execute();

            // Small delay before dealing first hand
            StartCoroutine(DelayedStart());
        }

        private void HandleCardSelected(Card card)
        {
            if (currentHand.Contains(card) && !selectedCards.Contains(card))
                selectedCards.Add(card);
        }
        private void HandleCardDeselected(Card card)
        {
            if (selectedCards.Contains(card))
                selectedCards.Remove(card);
        }

        private void HandleHandSubmitted(Card[] cards)
        {
            if (!gameActive) return;

            float handScore = CalculateHandScore(cards);
            currentScore += handScore;

            new ScoreUpdatedEvent(currentScore).Execute();

            // Check win condition
            if (currentScore >= targetScore)
            {
                gameActive = false;
                new GameStateChangedEvent("Victory!").Execute();
                return;
            }
        }

        private void HandleTimerExpired()
        {
            gameActive = false;
            new GameStateChangedEvent("Time's Up!").Execute();
        }

        private void HandleGameRestart()
        {
            InitializeGame();
        }

        private float CalculateHandScore(Card[] cards)
        {
            float baseScore = 0;

            // TODO - Implement card effects and scoring logic
            foreach (Card card in cards)
            {
                baseScore += card.GetValue();
            }

            // Apply multipliers based on hand type
            HandType handType = EvaluateHandType(cards);
            return baseScore * GetHandMultiplier(handType);
        }

        private int GetHandMultiplier(HandType handType)
        {
            // Return multiplier based on hand strength
            return 1; // Placeholder
        }

        private HandType EvaluateHandType(Card[] cards)
        {
            // Implement hand evaluation (pair, flush, straight, etc.)
            return HandType.HighCard; // Placeholder
        }

        private void DealNewHand()
        {
            Card[] newHand = DealCards(5);
            currentHand = new List<Card>(newHand);
            selectedCards.Clear();
            new NewHandDealtEvent(newHand).Execute();
        }

        private Card[] DealCards(int amount)
        {
            if (availableCards == null || amount <= 0)
            {
                return new Card[0];
            }

            List<Card> uniqueCards = availableCards
                .Where(card => card != null)
                .GroupBy(card => card.cardId)
                .Select(group => group.First())
                .ToList();

            if (uniqueCards.Count == 0)
            {
                return new Card[0];
            }

            if (amount > uniqueCards.Count)
            {
                Debug.LogWarning($"Requested {amount} cards but only {uniqueCards.Count} unique cards are available.");
                amount = uniqueCards.Count;
            }

            const int maxAttempts = 100;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                List<Card> shuffled = new List<Card>(uniqueCards);
                for (int i = shuffled.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
                }

                Card[] candidateHand = shuffled.Take(amount).ToArray();
                string handKey = BuildHandKey(candidateHand);

                if (dealtHandKeys.Add(handKey))
                {
                    return candidateHand;
                }
            }

            // All combinations have likely been used. Reset history and continue.
            dealtHandKeys.Clear();

            List<Card> resetShuffle = new List<Card>(uniqueCards);
            for (int i = resetShuffle.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (resetShuffle[i], resetShuffle[j]) = (resetShuffle[j], resetShuffle[i]);
            }

            Card[] fallbackHand = resetShuffle.Take(amount).ToArray();
            dealtHandKeys.Add(BuildHandKey(fallbackHand));
            return fallbackHand;
        }

        private static string BuildHandKey(Card[] hand)
        {
            int[] sortedIds = hand
                .Select(card => card.cardId)
                .OrderBy(id => id)
                .ToArray();

            return string.Join("-", sortedIds);
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(1f);
            new GameStateChangedEvent("Playing").Execute();
            DealNewHand();
        }

        // Public getters for UI
        public float GetCurrentScore() => currentScore;
        public float GetTimeRemaining() => roundTimeLimit - currentTime;
        public float GetTargetScore() => targetScore;
    }

    public enum HandType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
}
