using System;

namespace SpeedBalatro
{
    public static class SpeedBalatroEvents
    {
        public static event Action<Card> OnCardSelected;
        public static event Action<Card> OnCardDeselected;
        public static event Action<Card[]> OnHandSubmitted;
        public static event Action OnTimerExpired;
        public static event Action OnGameRestart;

        public static event Action<float> OnTimerUpdated;
        public static event Action<Card[]> OnNewHandDealt;
        public static event Action<string> OnGameStateChanged;
        public static event Action<float> OnScoreUpdated;
        public static event Action<GameManager.HandScoreInfo> OnHandScoreInfoUpdated;

        public static void RaiseCardSelected(Card card) => OnCardSelected?.Invoke(card);
        public static void RaiseCardDeselected(Card card) => OnCardDeselected?.Invoke(card);
        public static void RaiseHandSubmitted(Card[] cards) => OnHandSubmitted?.Invoke(cards);
        public static void RaiseTimerExpired() => OnTimerExpired?.Invoke();
        public static void RaiseGameRestart() => OnGameRestart?.Invoke();
        public static void RaiseTimerUpdated(float time) => OnTimerUpdated?.Invoke(time);
        public static void RaiseNewHandDealt(Card[] hand) => OnNewHandDealt?.Invoke(hand);
        public static void RaiseGameStateChanged(string state) => OnGameStateChanged?.Invoke(state);
        public static void RaiseScoreUpdated(float score) => OnScoreUpdated?.Invoke(score);
        public static void RaiseHandScoreInfoUpdated(GameManager.HandScoreInfo info) => OnHandScoreInfoUpdated?.Invoke(info);
    }
}
