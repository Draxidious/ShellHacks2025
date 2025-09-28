using UnityEngine;

public enum CardAction
{
    CollectFromAllPlayers,
    CollectFromBank,
    PayFine,
    AdvanceToNextProperty
}

[System.Serializable]
public class CommunityChestCard
{
    [TextArea(2, 5)]
    public string description;
    public CardAction action;
    public int amount;
}

[System.Serializable]
public class TriviaQuestion
{
    [TextArea(2, 3)]
    public string question;
    public string answer;
}
