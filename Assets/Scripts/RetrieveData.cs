using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads jobs.json at runtime and exposes maps:
///  - profession => List<CommunityChestCard>
///  - profession => List<TriviaQuestion>
/// Uses Resources.Load at path "json/jobs".
/// </summary>
public class JobsDataRepository : MonoBehaviour
{
    public static JobsDataRepository Instance { get; private set; }

    [Header("Resources path (no extension)")]
    [Tooltip("Resources.Load<TextAsset>(resourcesPath). Default: json/jobs (i.e., Assets/Resources/json/jobs.json)")]
    public string resourcesPath = "json/jobs";

    private readonly Dictionary<string, List<CommunityChestCard>> _eventsByJob =
        new Dictionary<string, List<CommunityChestCard>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<TriviaQuestion>> _triviaByJob =
        new Dictionary<string, List<TriviaQuestion>>(StringComparer.OrdinalIgnoreCase);

    [Serializable] private class JobsRoot { public List<JobJson> Jobs; }
    [Serializable]
    private class JobJson
    {
        public string Name;
        public List<EventJson> Events;
        public List<TriviaJson> Trivia;
    }
    [Serializable]
    private class EventJson
    {
        public string Description;
        public int Amount;
        public string Type;
    }
    [Serializable] private class TriviaJson { public string Question; public string Answer; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromResources();
    }

    private void LoadFromResources()
    {
        var ta = Resources.Load<TextAsset>(resourcesPath);
        if (ta == null)
        {
            Debug.LogError($"[JobsDataRepository] Could not find TextAsset at Resources path '{resourcesPath}'. " +
                           "Place your file at Assets/Resources/json/jobs.json or update resourcesPath.");
            return;
        }

        // Parse JSON
        var root = JsonUtility.FromJson<JobsRoot>(ta.text);
        if (root == null || root.Jobs == null || root.Jobs.Count == 0)
        {
            Debug.LogError("[JobsDataRepository] JSON parsed but no jobs found.");
            return;
        }

        _eventsByJob.Clear();
        _triviaByJob.Clear();

        foreach (var j in root.Jobs)
        {
            var key = (j.Name ?? "").Trim();
            if (string.IsNullOrEmpty(key)) continue;

            // Build Events list -> CommunityChestCard
            var evList = new List<CommunityChestCard>();
            if (j.Events != null)
            {
                foreach (var ev in j.Events)
                {
                    var card = new CommunityChestCard
                    {
                        description = ev.Description,
                        amount = ev.Amount,
                        action = MapAction(ev.Type)
                    };
                    evList.Add(card);
                }
            }
            _eventsByJob[key] = evList;

            // Build Trivia list -> TriviaQuestion
            var trList = new List<TriviaQuestion>();
            if (j.Trivia != null)
            {
                foreach (var t in j.Trivia)
                {
                    trList.Add(new TriviaQuestion
                    {
                        question = t.Question,
                        answer = t.Answer
                    });
                }
            }
            _triviaByJob[key] = trList;
        }

    }

    private static CardAction MapAction(string typeString)
    {
        if (string.IsNullOrWhiteSpace(typeString)) return CardAction.CollectFromBank;
        switch (typeString.Trim().ToLowerInvariant())
        {
            case "collect": return CardAction.CollectFromBank;
            case "pay": return CardAction.PayFine;
            // extend here if you add more types later, e.g. "advance"
            default: return CardAction.CollectFromBank;
        }
    }

    // -------------------- Public API --------------------

    public bool TryGetEvents(string profession, out List<CommunityChestCard> cards)
        => _eventsByJob.TryGetValue(profession ?? "", out cards);

    public bool TryGetTrivia(string profession, out List<TriviaQuestion> questions)
        => _triviaByJob.TryGetValue(profession ?? "", out questions);

    public CommunityChestCard GetRandomEvent(string profession)
    {
        if (TryGetEvents(profession, out var list) && list.Count > 0)
            return list[UnityEngine.Random.Range(0, list.Count)];

        // fallback to any job with events
        foreach (var kv in _eventsByJob)
            if (kv.Value != null && kv.Value.Count > 0)
                return kv.Value[UnityEngine.Random.Range(0, kv.Value.Count)];

        return null;
    }

    public TriviaQuestion GetRandomTrivia(string profession)
    {
        if (TryGetTrivia(profession, out var list) && list.Count > 0)
            return list[UnityEngine.Random.Range(0, list.Count)];

        // fallback to any job with trivia
        foreach (var kv in _triviaByJob)
            if (kv.Value != null && kv.Value.Count > 0)
                return kv.Value[UnityEngine.Random.Range(0, kv.Value.Count)];

        return null;
    }

    public string GetAnyProfessionWithEvents()
    {
        foreach (var kv in _eventsByJob)
            if (kv.Value != null && kv.Value.Count > 0)
                return kv.Key;
        return null;
    }

    public string GetAnyProfessionWithTrivia()
    {
        foreach (var kv in _triviaByJob)
            if (kv.Value != null && kv.Value.Count > 0)
                return kv.Key;
        return null;
    }
}
