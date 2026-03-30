using System;

namespace SpeedBalatro
{

    public abstract class GameEvent
    {
        public abstract void Execute();
    }

    // UI to GameManager Events
    public class CardSelectedEvent : GameEvent
    {
        public static event Action<Card> OnCardSelected;
        public Card selectedCard;

        public CardSelectedEvent(Card card)
        {
            selectedCard = card;
        }

        public override void Execute()
        {
            OnCardSelected?.Invoke(selectedCard);
        }
    }

        public class CardDeselectedEvent : GameEvent
    {
        public static event Action<Card> OnCardDeselected;
        public Card selectedCard;

        public CardDeselectedEvent(Card card)
        {
            selectedCard = card;
        }

        public override void Execute()
        {
            OnCardDeselected?.Invoke(selectedCard);
        }
    }

    public class HandSubmittedEvent : GameEvent
    {
        public static event Action<Card[]> OnHandSubmitted;
        public Card[] submittedCards;

        public HandSubmittedEvent(Card[] cards)
        {
            submittedCards = cards;
        }

        public override void Execute()
        {
            OnHandSubmitted?.Invoke(submittedCards);
        }
    }

    public class TimerExpiredEvent : GameEvent
    {
        public static event Action OnTimerExpired;

        public override void Execute()
        {
            OnTimerExpired?.Invoke();
        }
    }

    public class GameRestartEvent : GameEvent
    {
        public static event Action OnGameRestart;

        public override void Execute()
        {
            OnGameRestart?.Invoke();
        }
    }

    // GameManager to UI Events
    public class ScoreUpdatedEvent : GameEvent
    {
        public static event Action<float> OnScoreUpdated;
        public float newScore;

        public ScoreUpdatedEvent(float score)
        {
            newScore = score;
        }

        public override void Execute()
        {
            OnScoreUpdated?.Invoke(newScore);
        }
    }

    public class TimerUpdatedEvent : GameEvent
    {
        public static event Action<float> OnTimerUpdated;
        public float timeRemaining;

        public TimerUpdatedEvent(float time)
        {
            timeRemaining = time;
        }

        public override void Execute()
        {
            OnTimerUpdated?.Invoke(timeRemaining);
        }
    }

    public class NewHandDealtEvent : GameEvent
    {
        public static event Action<Card[]> OnNewHandDealt;
        public Card[] newHand;

        public NewHandDealtEvent(Card[] cards)
        {
            newHand = cards;
        }

        public override void Execute()
        {
            OnNewHandDealt?.Invoke(newHand);
        }
    }

    public class GameStateChangedEvent : GameEvent
    {
        public static event Action<string> OnGameStateChanged;
        public string newState;

        public GameStateChangedEvent(string state)
        {
            newState = state;
        }

        public override void Execute()
        {
            OnGameStateChanged?.Invoke(newState);
        }
    }
}