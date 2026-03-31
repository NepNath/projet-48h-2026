using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace SpeedBalatro
{

    public class GameManager : MonoBehaviour
    {
        public struct HandScoreInfo
        {
            public HandType handType;
            public float chips;
            public int mult;
            public float totalScore;
        }

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


        void Start()
        {
            InitializeGame();
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            SpeedBalatroEvents.OnCardSelected += HandleCardSelected;
            SpeedBalatroEvents.OnCardDeselected += HandleCardDeselected;
            SpeedBalatroEvents.OnHandSubmitted += HandleHandSubmitted;
            SpeedBalatroEvents.OnTimerExpired += HandleTimerExpired;
            SpeedBalatroEvents.OnGameRestart += HandleGameRestart;
        }

        private void OnDestroy()
        {
            SpeedBalatroEvents.OnCardSelected -= HandleCardSelected;
            SpeedBalatroEvents.OnCardDeselected -= HandleCardDeselected;
            SpeedBalatroEvents.OnHandSubmitted -= HandleHandSubmitted;
            SpeedBalatroEvents.OnTimerExpired -= HandleTimerExpired;
            SpeedBalatroEvents.OnGameRestart -= HandleGameRestart;
        }

        private void UpdateTimer()
        {
            currentTime -= Time.deltaTime;
            SpeedBalatroEvents.RaiseTimerUpdated(currentTime);
            if (currentTime <= 0)
            {
                SpeedBalatroEvents.RaiseTimerExpired();
            }
        }

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

            SpeedBalatroEvents.RaiseTimerUpdated(currentTime);
            SpeedBalatroEvents.RaiseGameStateChanged("Get Ready!");
            SpeedBalatroEvents.RaiseScoreUpdated(currentScore);

            StartCoroutine(DelayedStart());
        }

        private void HandleCardSelected(Card card)
        {
            if (currentHand.Contains(card) && !selectedCards.Contains(card))
                selectedCards.Add(card);

            HandScoreInfo info = GetHandScoreInfo(selectedCards.ToArray());
            SpeedBalatroEvents.RaiseHandScoreInfoUpdated(info);
            SpeedBalatroEvents.RaiseScoreUpdated(info.totalScore);
        }

        private void HandleCardDeselected(Card card)
        {
            if (selectedCards.Contains(card))
                selectedCards.Remove(card);

            HandScoreInfo info = GetHandScoreInfo(selectedCards.ToArray());
            SpeedBalatroEvents.RaiseHandScoreInfoUpdated(info);
            SpeedBalatroEvents.RaiseScoreUpdated(info.totalScore);
        }



        private void HandleHandSubmitted(Card[] cards)
        {
            if (!gameActive) return;

            float handScore = CalculateHandScore(cards);
            currentScore += handScore;
            SpeedBalatroEvents.RaiseScoreUpdated(currentScore);

            if (currentScore == targetScore)
            {
                gameActive = false;
                SpeedBalatroEvents.RaiseGameStateChanged("Perfect Score!");
                return;
            }
            else if (currentScore > targetScore)
            {
                gameActive = false;
                SpeedBalatroEvents.RaiseGameStateChanged("Bust! Too High!");
                return;
            }

            selectedCards.Clear();
            DealNewHand();
        }

        private void HandleTimerExpired()
        {
            gameActive = false;
            SpeedBalatroEvents.RaiseGameStateChanged("Time's Up!");
            SpeedBalatroEvents.RaiseScoreUpdated(currentScore);
        }

        private void HandleGameRestart()
        {
            dealtHandKeys.Clear();
            InitializeGame();
        }

        private float CalculateHandScore(Card[] cards)
        {
            return GetHandScoreInfo(cards).totalScore;
        }

        private float CalculateMaxScoreFromHand(Card[] hand)
        {
            float maxScore = 0f;
            int totalCombinations = 1 << hand.Length; // 2^n combinations

            for (int i = 1; i < totalCombinations; i++)
            {
                List<Card> subset = new();
                for (int j = 0; j < hand.Length; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        subset.Add(hand[j]);
                    }
                }

                if (subset.Count > 0)
                {
                    float score = GetHandScoreInfo(subset.ToArray()).totalScore;
                    if (score > maxScore)
                    {
                        maxScore = score;
                    }
                }
            }

            return maxScore;
        }
        private int GetHandMultiplier(HandType handType)
        {
            return handType switch
            {
                HandType.HighCard => 1,
                HandType.Pair => 2,
                HandType.TwoPair => 2,
                HandType.ThreeOfAKind => 3,
                HandType.Straight => 4,
                HandType.Flush => 4,
                HandType.FullHouse => 4,
                HandType.FourOfAKind => 7,
                HandType.StraightFlush => 8,
                HandType.RoyalFlush => 8,
                _ => 1
            };
        }

        private float GetHandBaseChips(HandType handType)
        {
            return handType switch
            {
                HandType.HighCard => 5,
                HandType.Pair => 10,
                HandType.TwoPair => 20,
                HandType.ThreeOfAKind => 30,
                HandType.Straight => 30,
                HandType.Flush => 35,
                HandType.FullHouse => 40,
                HandType.FourOfAKind => 60,
                HandType.StraightFlush => 100,
                HandType.RoyalFlush => 100,
                _ => 0
            };
        }

        private HandType EvaluateHandType(Card[] cards)
        {
            if (cards == null || cards.Length == 0)
                return HandType.HighCard;

            bool isFlush = cards.Length >= 5
                && cards.All(c => c.suit == cards[0].suit);
            bool isStraight = IsStraight(cards);

            var rankGroups = cards
                .GroupBy(c => c.rank)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            int largest = rankGroups[0].Count();
            int secondLargest = rankGroups.Count > 1
                ? rankGroups[1].Count()
                : 0;

            if (isFlush && isStraight)
            {
                int[] sorted = cards
                    .Select(c => (int)c.rank)
                    .OrderBy(r => r)
                    .ToArray();

                bool isRoyal = sorted.SequenceEqual(
                    new[] { 1, 10, 11, 12, 13 }
                );
                return isRoyal ? HandType.RoyalFlush : HandType.StraightFlush;
            }

            if (largest == 4) return HandType.FourOfAKind;
            if (largest == 3 && secondLargest >= 2) return HandType.FullHouse;
            if (isFlush) return HandType.Flush;
            if (isStraight) return HandType.Straight;
            if (largest == 3) return HandType.ThreeOfAKind;
            if (largest == 2 && secondLargest == 2) return HandType.TwoPair;
            if (largest == 2) return HandType.Pair;

            return HandType.HighCard;
        }

        private bool IsStraight(Card[] cards)
        {
            if (cards == null || cards.Length < 5) return false;

            int[] ranks = cards
                .Select(c => (int)c.rank)
                .Distinct()
                .OrderBy(r => r)
                .ToArray();

            if (ranks.Length < 5) return false;

            // Normal consecutive: e.g. 3-4-5-6-7
            if (ranks[4] - ranks[0] == 4) return true;

            // Ace-high wrap: A(1), 10, J, Q, K
            if (ranks.SequenceEqual(new[] { 1, 10, 11, 12, 13 }))
                return true;

            return false;
        }

        /// Returns only the cards that "score" for the given hand type,
        /// matching Balatro's behaviour.
        private Card[] GetScoringCards(Card[] cards, HandType handType)
        {
            switch (handType)
            {
                case HandType.Straight:
                case HandType.Flush:
                case HandType.FullHouse:
                case HandType.StraightFlush:
                case HandType.RoyalFlush:
                    return cards; // all cards score

                case HandType.FourOfAKind:
                    {
                        CardRank quadRank = cards
                            .GroupBy(c => c.rank)
                            .First(g => g.Count() == 4)
                            .Key;
                        return cards.Where(c => c.rank == quadRank).ToArray();
                    }

                case HandType.ThreeOfAKind:
                    {
                        CardRank tripRank = cards
                            .GroupBy(c => c.rank)
                            .First(g => g.Count() == 3)
                            .Key;
                        return cards.Where(c => c.rank == tripRank).ToArray();
                    }

                case HandType.TwoPair:
                    {
                        var pairRanks = cards
                            .GroupBy(c => c.rank)
                            .Where(g => g.Count() >= 2)
                            .OrderByDescending(g => g.Key)
                            .Take(2)
                            .Select(g => g.Key)
                            .ToHashSet();
                        return cards.Where(c => pairRanks.Contains(c.rank)).ToArray();
                    }

                case HandType.Pair:
                    {
                        CardRank pairRank = cards
                            .GroupBy(c => c.rank)
                            .First(g => g.Count() == 2)
                            .Key;
                        return cards.Where(c => c.rank == pairRank).ToArray();
                    }

                case HandType.HighCard:
                default:
                    {
                        // Only the single highest card scores.
                        // Ace is highest in Balatro.
                        Card highest = cards
                            .OrderByDescending(c =>
                                c.rank == CardRank.Ace ? 14 : (int)c.rank
                            )
                            .First();
                        return new[] { highest };
                    }
            }
        }
        public HandScoreInfo GetHandScoreInfo(Card[] cards)
        {
            if (cards == null || cards.Length == 0)
            {
                return new HandScoreInfo
                {
                    handType = HandType.HighCard,
                    chips = 0,
                    mult = 0,
                    totalScore = 0
                };
            }

            HandType handType = EvaluateHandType(cards);
            float baseChips = GetHandBaseChips(handType);
            int mult = GetHandMultiplier(handType);

            Card[] scoringCards = GetScoringCards(cards, handType);
            float cardChips = 0f;
            foreach (Card card in scoringCards)
            {
                cardChips += card.GetValue();
            }

            float totalChips = baseChips + cardChips;

            return new HandScoreInfo
            {
                handType = handType,
                chips = totalChips,
                mult = mult,
                totalScore = totalChips * mult
            };
        }



        private void DealNewHand()
        {
            Card[] newHand = DealCards(5);
            currentHand = new List<Card>(newHand);
            selectedCards.Clear();

            float maxScore = CalculateMaxScoreFromHand(newHand);
            targetScore = maxScore;
            SpeedBalatroEvents.RaiseTargetScoreChanged(targetScore);

            SpeedBalatroEvents.RaiseNewHandDealt(newHand);
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
            DealNewHand();
            yield return new WaitForSeconds(0.6f);
            gameActive = true;
            SpeedBalatroEvents.RaiseGameStateChanged("Playing");
        }

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
