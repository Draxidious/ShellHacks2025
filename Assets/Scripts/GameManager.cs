using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene-level GameManager. Attach this to a GameObject in your starting scene.
/// Provides basic data for up to 3 players and simple helper methods.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Optional singleton instance (useful if you want a single manager across scenes)
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string playerName = "Player";
        public bool isActive = true;

        public bool isTurnActive = false;
        public float score = 0;
        public Vector3 spawnPosition = Vector3.zero;

        // convenience
        public override string ToString()
        {
            return $"[{id}] {playerName} (H, Score: {score})";
        }
    }

    [Header("Players (exactly 3)")]
    [Tooltip("Basic player data for up to 3 players. OnValidate will ensure there are 3 entries.")]
    public List<PlayerData> players = new List<PlayerData>();

    [Header("Game Settings")]
    public bool dontDestroyOnLoad = false;

    [SerializeField] float startScore = 1000f;

    public int debugPlayerIndex = 0;
    public int debugMove = 0;
    public bool debugMovement = false;

    void Awake()
    {
        // Setup singleton
        if (Instance == null)
        {
            Instance = this;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // If another instance exists, destroy this one to enforce single manager
            Destroy(gameObject);
            return;
        }

        // Ensure we have exactly 3 players configured
        EnsureThreePlayers();
    }

    private void Start()
    {
        ResetPlayers();
    }
    private void Update()
    {
        if (debugMovement && debugMove != 0)
        {
            debugMovement = false;
            RequestMovePlayer(debugPlayerIndex, (int)debugMove);
        }
    }


    // Ensure there are exactly 3 player entries (used in editor and at runtime)
    private void EnsureThreePlayers()
    {
        while (players.Count < 3)
        {
            PlayerData p = new PlayerData();
            p.id = players.Count + 1;
            p.playerName = $"Player{p.id}";
            players.Add(p);
        }

        // If more than 3, trim extras (keep first 3)
        if (players.Count > 3)
        {
            players.RemoveRange(3, players.Count - 3);
        }

        // Make sure ids are consistent
        for (int i = 0; i < players.Count; i++)
        {
            players[i].id = i + 1;
            if (string.IsNullOrEmpty(players[i].playerName))
                players[i].playerName = $"Player{players[i].id}";
        }
    }

    // Called when values change in the inspector
    private void OnValidate()
    {
        EnsureThreePlayers();
    }



    #region Public API
    // Public API: get player by id (1-based)
    public PlayerData GetPlayer(int id)
    {
        if (id < 1 || id > players.Count) return null;
        return players[id - 1];
    }

    // Activate/deactivate a player
    public void SetPlayerActive(int id, bool active)
    {
        PlayerData p = GetPlayer(id);
        if (p != null) p.isActive = active;
    }

    // Add score to a player
    public void AddScore(int id, int amount)
    {
        PlayerData p = GetPlayer(id);
        if (p != null) p.score += amount;
    }

    // Reset all players to default values
    public void ResetPlayers()
    {
        foreach (PlayerData p in players)
        {
            p.score = startScore;
            p.isActive = true;
        }
    }
    
    #endregion Public API

    public void TurnHandler()
    {
        
    }

    // Event-based API: request that a player piece be moved. Parameters: playerIndex (0-based), boardSpaceIndex (0-based)
    public static Action<int, int> OnPlayerMoveRequested;

    // Call this to request a move. This will notify any subscribers (e.g., BoardManager).
    public void RequestMovePlayer(int playerIndex, int boardSpaceIndex)
    {
        OnPlayerMoveRequested?.Invoke(playerIndex, boardSpaceIndex);
    }

    // Small debug helper to print all players
    [ContextMenu("Print Players To Console")]
    public void DebugPrintPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].ToString());
        }
    }
}
