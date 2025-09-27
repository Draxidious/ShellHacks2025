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
    HashSet<string> jobSet = new HashSet<string>
    {
        "Software Engineer",
        "Doctor"
    };

    public const int maxPlayers = 4;
    
    [Header("Players")]
    [Tooltip("Basic player data for up to 4 players. OnValidate will ensure there are 4 entries.")]
    public List<Player> players = new List<Player>();

    [Header("Game Settings")]
    public bool dontDestroyOnLoad = false;

    public int debugPlayerIndex = 0;
    public int movePlayerSpaces = 0;
    public bool movePlayerForward = false;

    public bool addTestPlayers = true;

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
    }

    private void Start()
    {
        if (addTestPlayers)
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                Player newPlayer = new Player
                {
                    id = i + 1,
                    playerName = $"Player {i + 1}",
                    money = 0
                };
                AddPlayer(newPlayer);
            }
        }
    }

    public void AddPlayer(Player newPlayer)
    {
        if (players.Count >= maxPlayers)
        {
            Debug.LogWarning("Maximum number of players reached. Cannot add more players.");
            return;
        }
        players.Add(newPlayer);
    }
    private void Update()
    {
        if (movePlayerForward && movePlayerSpaces != 0)
        {
            movePlayerForward = false;
            RequestMovePlayer(debugPlayerIndex, movePlayerSpaces);
        }
    }


    #region Public API
    // Public API: get player by id (1-based)
    public Player GetPlayer(int id)
    {
        if (id < 1 || id > players.Count) return null;
        return players[id - 1];
    }
    
    #endregion Public API

    // Event-based API: request that a player piece be moved. Parameters: playerIndex (0-based), boardSpaceIndex (0-based)
    public static Action<int, int> OnPlayerMoveRequested;

    // Call this to request a move. This will notify any subscribers (e.g., BoardManager).
    public void RequestMovePlayer(int playerIndex, int boardSpaceIndex)
    {
        OnPlayerMoveRequested?.Invoke(playerIndex, boardSpaceIndex);
    }

    [ContextMenu("Print Players To Console")]
    public void DebugPrintPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].ToString());
        }
    }
}
