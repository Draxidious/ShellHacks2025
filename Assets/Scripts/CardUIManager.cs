using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    public static CardUIManager Instance { get; private set; }

    [Header("Data")]
    public GameDataManager dataManager;

    [Header("Parents (spawn under these)")]
    public Transform triviaPanelParent;
    public Transform eventPanelParent;

    [Header("Prefabs")]
    public TriviaCardUI triviaCardPrefab;
    public EventCardUI eventCardPrefab;

    [Header("Optional")]
    [SerializeField] bool logSpawns = true;

    void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }

        GameManager.OnTriviaLandedOn += HandleTriviaLandedOn;
        GameManager.OnEventLandedOn  += HandleEventLandedOn;
    }

    void Start()
    {
        ShowRandomTriviaForCurrentPlayer(null);
        ShowRandomEventForCurrentPlayer(null);
    }

    void OnDestroy()
    {
        GameManager.OnTriviaLandedOn -= HandleTriviaLandedOn;
        GameManager.OnEventLandedOn  -= HandleEventLandedOn;
    }

    private void HandleTriviaLandedOn()
    {
        if (logSpawns) Debug.Log("[CardUIManager] Trivia landed -> spawning card");
        ShowRandomTriviaForCurrentPlayer(null);
    }

    private void HandleEventLandedOn()
    {
        if (logSpawns) Debug.Log("[CardUIManager] Event landed -> spawning card");
        ShowRandomEventForCurrentPlayer(null);
    }

    public void ShowRandomTriviaForCurrentPlayer(System.Action onClosed)
    {
        var job = GetCurrentPlayerJob();
        var q = PickRandomTrivia(job);
        var ui = Instantiate(triviaCardPrefab, triviaPanelParent);
        ui.Show(q, onClosed ?? DefaultClose);
    }

    public void ShowRandomEventForCurrentPlayer(System.Action onResolved)
    {
        var job = GetCurrentPlayerJob();
        var card = PickRandomEvent(job);
        var ui = Instantiate(eventCardPrefab, eventPanelParent);
        ui.Show(card, onResolved ?? DefaultClose);
    }

    private void DefaultClose()
    {
        // Do nothing: we promised not to change GameState flow.
        // (If you want: uncomment to advance turns automatically)
        // GameManager.Instance.NextTurn();
    }

    private JobData GetCurrentPlayerJob()
    {
        var p = GameManager.players[GameManager.Instance.currentTurnPlayerIndex];
        JobData job = null;
        if (dataManager != null) job = dataManager.GetJob(p.profession);
        if (job == null && dataManager != null && dataManager.jobs != null && dataManager.jobs.Length > 0)
        {
            job = dataManager.jobs[Random.Range(0, dataManager.jobs.Length)];
            Debug.LogWarning($"[CardUIManager] No JobData for '{p.profession}', using fallback: {job?.jobName}");
        }
        return job;
    }

    private TriviaQuestion PickRandomTrivia(JobData job)
    {
        if (job == null || job.triviaQuestions == null || job.triviaQuestions.Count == 0)
            return new TriviaQuestion { question = "No trivia available.", answer = "N/A" };
        return job.triviaQuestions[Random.Range(0, job.triviaQuestions.Count)];
    }

    private CommunityChestCard PickRandomEvent(JobData job)
    {
        if (job == null || job.communityChestCards == null || job.communityChestCards.Count == 0)
            return new CommunityChestCard { description = "No event available.", action = CardAction.CollectFromBank, amount = 0 };
        return job.communityChestCards[Random.Range(0, job.communityChestCards.Count)];
    }

    // Handy manual test from the Inspector (no GameState changes):
    [ContextMenu("Spawn Test Trivia")]
    private void _SpawnTestTrivia() => ShowRandomTriviaForCurrentPlayer(null);

    [ContextMenu("Spawn Test Event")]
    private void _SpawnTestEvent() => ShowRandomEventForCurrentPlayer(null);
}
