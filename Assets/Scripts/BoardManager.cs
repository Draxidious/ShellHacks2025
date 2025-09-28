using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Singleton instance for easy global access
    public static BoardManager Instance { get; private set; }

    #region Board Tiles
    public GameObject TileObjects;
    public GameObject PieceObjects;
    public List<Tile> Tiles;
    #endregion

    #region Player variables
    private List<GameObject> playerPieces;
    #endregion

    [SerializeField] private float initDelaySeconds = 3f;  // time to wait before initializing board
    [SerializeField] private float moveDelaySeconds = 2f;   // time to wait before moving after dice roll
    private bool _initialized = false;

    // ----------------------------
    // Lifecycle
    // ----------------------------
    private void Awake()
    {
        // Singleton guard
        if (Instance == null) Instance = this;
        else if (Instance != this) { Destroy(gameObject); return; }

        // Subscribe here to core signals
        GameManager.OnDiceRolled += HandleDiceRolled;
        GameManager.OnMoneyUpdated += HandleBankruptcy;
    }

    private void Start()
    {
        GameManager.OnPlayerAdded += AddPlayerPiece;
        StartCoroutine(InitBoard());
    }

    private void OnEnable()
    {
        GameManager.OnPlayerMoveRequested += HandlePlayerMoveRequested;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerMoveRequested -= HandlePlayerMoveRequested;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerAdded         -= AddPlayerPiece;
        GameManager.OnDiceRolled          -= HandleDiceRolled;
        GameManager.OnMoneyUpdated        -= HandleBankruptcy;
        GameManager.OnPlayerMoveRequested -= HandlePlayerMoveRequested;
    }

    private void Update() { }

    // ----------------------------
    // Init & Guards
    // ----------------------------
    private System.Collections.IEnumerator InitBoard()
    {
        // Optional staging delay (e.g., waiting for other systems to boot)
        if (initDelaySeconds > 0f)
            yield return new WaitForSeconds(initDelaySeconds);

        GetTilesAndPieces();

        // Defensive: ensure lists exist
        if (playerPieces == null) playerPieces = new List<GameObject>();
        if (Tiles == null) Tiles = new List<Tile>();

        _initialized = true; // mark ready

        // Position all existing pieces at start
        for (int i = 0; i < playerPieces.Count; i++)
        {
            MovePlayerPiece(i, 0);
        }
    }

    private System.Collections.IEnumerator WaitUntilReady()
    {
        while (!_initialized) yield return null;
    }

    // ----------------------------
    // Event Handlers
    // ----------------------------
    public void HandleBankruptcy(int playerId, int amount)
    {
        int playerIndex = playerId - 1;

        if (GameManager.players == null ||
            playerIndex < 0 ||
            playerIndex >= GameManager.players.Count)
        {
            Debug.LogWarning($"HandleBankruptcy: invalid player index {playerIndex}.");
            return;
        }

        Player player = GameManager.players[playerIndex];
        if (player == null) return;

        if (player.money < 0)
        {
            Debug.Log($"Player {playerIndex + 1} is bankrupt and removed from the game.");

            if (playerPieces != null &&
                playerIndex >= 0 &&
                playerIndex < playerPieces.Count &&
                playerPieces[playerIndex] != null)
            {
                playerPieces[playerIndex].SetActive(false);
            }

            if (Tiles != null)
            {
                foreach (Tile tile in Tiles)
                {
                    if (tile != null &&
                        tile.tileType == TileType.Property &&
                        tile.playerOwnerId == playerId)
                    {
                        tile.setOwner(-1);
                        Debug.Log($"Released ownership of {tile.tileName} from Player {playerId}");
                    }
                }
            }
        }
    }

    public void AddPlayerPiece(Player player)
    {
        if (player == null) return;

        // If added early, try to prepare the lists now
        if (!_initialized) GetTilesAndPieces();

        int playerIndex = player.id - 1;

        if (playerPieces == null ||
            playerIndex < 0 ||
            playerIndex >= playerPieces.Count ||
            playerPieces[playerIndex] == null)
        {
            Debug.LogWarning($"AddPlayerPiece: invalid index {playerIndex} or playerPieces not ready.");
            return;
        }

        if (Tiles == null || Tiles.Count == 0 || Tiles[0] == null)
        {
            Debug.LogWarning("AddPlayerPiece: Tiles not ready.");
            return;
        }

        playerPieces[playerIndex].transform.position = Tiles[0].GetPiecePlacementPosition(playerIndex);
    }

    public void HandleDiceRolled(int sum)
    {
        StartCoroutine(MovePlayerByDiceAmount(sum));
    }

    public System.Collections.IEnumerator MovePlayerByDiceAmount(int sum)
    {
        if (moveDelaySeconds > 0f)
            yield return new WaitForSeconds(moveDelaySeconds);

        // Ensure board data is ready
        yield return WaitUntilReady();

        if (GameManager.Instance == null)
        {
            Debug.LogError("MovePlayerByDiceAmount: GameManager.Instance is null.");
            yield break;
        }

        Debug.Log($"Moving player {GameManager.Instance.currentTurnPlayerIndex} by {sum} spaces.");
        MovePlayerPiece(GameManager.Instance.currentTurnPlayerIndex, sum);
    }

    private void HandlePlayerMoveRequested(int playerIndex, int boardSpaceIndex)
    {
        MovePlayerPiece(playerIndex, boardSpaceIndex);
    }

    // ----------------------------
    // Movement
    // ----------------------------
    // player index is player id - 1
    public void MovePlayerPiece(int playerIndex, int spaceMovement)
    {
        // Initialization checks
        if (Tiles == null || playerPieces == null)
        {
            Debug.LogError("MovePlayerPiece: Tiles or playerPieces not initialized.");
            return;
        }

        // Range & null checks
        if (playerIndex < 0 || playerIndex >= playerPieces.Count)
        {
            Debug.LogError($"MovePlayerPiece: player index {playerIndex} out of range (count={playerPieces.Count}).");
            return;
        }

        if (GameManager.players == null ||
            playerIndex >= GameManager.players.Count ||
            GameManager.players[playerIndex] == null)
        {
            Debug.LogError("MovePlayerPiece: GameManager.players not ready or target player is null.");
            return;
        }

        if (Tiles.Count == 0)
        {
            Debug.LogError("MovePlayerPiece: Tiles is empty.");
            return;
        }

        // Compute new position
        int currentPosition = GameManager.players[playerIndex].currentTileIndex;
        int newPosition = Tiles.Count > 0 ? (currentPosition + spaceMovement) % Tiles.Count : 0;
        if (newPosition < 0) newPosition += Tiles.Count;

        // Passed Start
        if (newPosition < currentPosition)
        {
            GameManager.Instance.UpdatePlayerMoney(playerIndex + 1, 100); // +1 because playerId = index + 1
            Debug.Log($"Player {playerIndex + 1} passed Start and collected $100.");
        }

        GameManager.players[playerIndex].currentTileIndex = newPosition;

        // Tile and piece safety
        Tile newTile = Tiles[newPosition];
        if (newTile == null)
        {
            Debug.LogError($"MovePlayerPiece: Tiles[{newPosition}] is null.");
            return;
        }

        var piece = playerPieces[playerIndex];
        if (piece == null)
        {
            Debug.LogError($"MovePlayerPiece: playerPieces[{playerIndex}] is null.");
            return;
        }

        // Move animation (if component exists), then snap as fallback
        Vector3 targetPosition = newTile.GetPiecePlacementPosition(playerIndex);
        var mover = piece.GetComponent<MovingBehavior>();
        if (mover != null)
        {
            mover.StartCoroutine(mover.HopToPosition(targetPosition));
        }
        else
        {
            Debug.LogWarning($"MovePlayerPiece: Missing MovingBehavior on piece {playerIndex}, snapping to position.");
        }
        piece.transform.position = targetPosition;

        // Trigger tile effect
        switch (newTile.tileType)
        {
            case TileType.Property:
                Debug.Log($"Player {playerIndex + 1} landed on a Property tile.");
                GameManager.OnPropertyLandedOn?.Invoke(newTile);
                break;
            case TileType.Trivia:
                Debug.Log($"Player {playerIndex + 1} landed on a Trivia tile.");
                GameManager.OnTriviaLandedOn?.Invoke();
                break;
            case TileType.Event:
                Debug.Log($"Player {playerIndex + 1} landed on an Event tile.");
                GameManager.OnEventLandedOn?.Invoke();
                GameManager.Instance.NextTurn();
                break;
            case TileType.Start:
            case TileType.Corner:
                GameManager.Instance.NextTurn();
                break;
            default:
                Debug.Log($"Player {playerIndex + 1} landed on a tile of type {newTile.tileType}.");
                break;
        }
    }

    // ----------------------------
    // Data gathering
    // ----------------------------
    private void GetTilesAndPieces()
    {
        // Tiles
        if (TileObjects == null)
        {
            Debug.LogError("GetTilesAndPieces: TileObjects is not assigned.");
            Tiles = new List<Tile>();
        }
        else
        {
            Tile[] tileArray = TileObjects.GetComponentsInChildren<Tile>(true);
            Tiles = new List<Tile>(tileArray);
        }

        // Player pieces
        playerPieces = new List<GameObject>();
        if (PieceObjects == null)
        {
            Debug.LogError("GetTilesAndPieces: PieceObjects is not assigned.");
            return;
        }

        int count = 0;
        foreach (Transform child in PieceObjects.transform)
        {
            if (child == null) continue;
            child.gameObject.SetActive(true);
            playerPieces.Add(child.gameObject);
            count++;
            if (GameManager.players != null && GameManager.players.Count == count)
                break;
        }

        if (GameManager.players != null && playerPieces.Count < GameManager.players.Count)
        {
            Debug.LogWarning($"GetTilesAndPieces: playerPieces found ({playerPieces.Count}) < players in GameManager ({GameManager.players.Count}).");
        }
    }
}
