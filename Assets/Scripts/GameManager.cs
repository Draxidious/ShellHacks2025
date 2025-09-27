using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene-level GameManager. Attach this to a GameObject in your starting scene.
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
    public static List<Player> players = new List<Player>();

    [Header("Game Settings")]

    // Event-based API: request that a player piece be moved. Parameters: playerIndex (0-based), boardSpaceIndex (0-based)
    public static Action<int, int> OnPlayerMoveRequested;
    public static Action<Player> OnPlayerAdded;
    public static Action OnTurnChanged;
    public static Action<int> OnDiceRolled;

    public bool dontDestroyOnLoad = false;

    public int debugPlayerIndex = 0;
    public int movePlayerSpaces = 0;
    public bool movePlayerForward = false;

    public bool addTestPlayers = true;

    [Header("Game Information")]

    public int currentTurnPlayerIndex = -1;

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

        if (addTestPlayers)
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                Player newPlayer = new Player
                {
                    id = i + 1,
                    profession = "Unemployed",
                    playerName = $"Player {i + 1}",
                    money = 0
                };
                AddPlayer(newPlayer);
            }
            
        }
        
    }

    // We want to start the next turn after everything is awake
    void Start()
    {
        NextTurn();
    }
    public void RequestMovePlayer(int playerIndex, int boardSpaceIndex)
    {
        OnPlayerMoveRequested?.Invoke(playerIndex, boardSpaceIndex);
    }

    public void AddPlayer(Player newPlayer)
    {
        OnPlayerAdded?.Invoke(newPlayer);
        players.Add(newPlayer);
    }

    public void DiceRolled(int sum)
    {
        OnDiceRolled?.Invoke(sum);
    }

    public void NextTurn()
    {
        Debug.Log("It is next turn");
        currentTurnPlayerIndex = (currentTurnPlayerIndex + 1) % players.Count;
        OnTurnChanged?.Invoke();
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

    [ContextMenu("Print Players To Console")]
    public void DebugPrintPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log(players[i].ToString());
        }
    }
}
