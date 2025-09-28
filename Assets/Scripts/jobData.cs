using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewJob", menuName = "Careeropoly/Job")]
public class JobData : ScriptableObject
{
    public string jobName;

    [Header("Community Chest Cards")]
    public List<CommunityChestCard> communityChestCards = new List<CommunityChestCard>();

    [Header("Trivia Questions")]
    public List<TriviaQuestion> triviaQuestions = new List<TriviaQuestion>();
}
