using UnityEngine;

[System.Serializable]
public enum CardSuit
{
    Hearts,
    Diamonds, 
    Clubs,
    Spades
}

[System.Serializable]
public enum CardRank
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

[System.Serializable]
public enum CardEffect
{
    None,
    Double,
    Triple,
    AddBonus,
    Multiply,
    WildCard
}

[CreateAssetMenu(fileName = "New Card", menuName = "Mini-game/SpeedBalatroCardFactory", order = 1)]
public class Card : ScriptableObject
{
    [Header("Card Identity")]
    public string cardName;
    public Sprite cardSprite;
    public int cardId;

    [Header("Card Attributes")]
    public CardSuit suit;
    public CardRank rank;
    public float baseValue;

    [Header("Card Effects")]
    public bool isSpecialCard;
    public CardEffect effect;
    public float effectMultiplier = 1;

    [Header("UI")]
    public Color cardColor = Color.white;

    public float GetValue() => baseValue * effectMultiplier;
    public bool IsRed() => suit == CardSuit.Hearts || suit == CardSuit.Diamonds;
    public bool IsBlack() => suit == CardSuit.Clubs || suit == CardSuit.Spades;
}