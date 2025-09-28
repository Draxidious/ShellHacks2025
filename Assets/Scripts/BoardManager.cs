using System;
using UnityEngine;
using System.Collections.Generic;

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
    List<GameObject> playerPieces;

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.OnPlayerAdded += AddPlayerPiece;

        StartCoroutine(InitBoard());
    }

    private System.Collections.IEnumerator InitBoard()
    {
        yield return new WaitForSeconds(3f); // wait 3 seconds
        GetTilesAndPieces();

        for (int i = 0; i < playerPieces.Count; i++)
        {
            MovePlayerPiece(i, 0);
        }
    }

    private void Awake()
    {
        GameManager.OnDiceRolled += HandleDiceRolled;
        GameManager.OnMoneyUpdated += HandleBankruptcy;

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void HandleBankruptcy(int playerId, int amount)
    {
        int playerIndex = playerId - 1;
        Player player = GameManager.players[playerIndex];
        if (player == null) return;
        if (player.money < 0)
        {
            Debug.Log($"Player {playerIndex} is bankrupt and removed from the game.");
            playerPieces[playerIndex].SetActive(false);

            foreach (Tile tile in Tiles)
            {
                if (tile.tileType == TileType.Property && tile.playerOwnerId == playerId)
                {
                    tile.setOwner(-1);
                    Debug.Log($"Released ownership of {tile.tileName} from Player {playerId}");
                }
            }
        }
    }

    public void AddPlayerPiece(Player player)
    {
        int playerIndex = player.id - 1;
        playerPieces[playerIndex].transform.position = Tiles[0].GetPiecePlacementPosition(playerIndex);
    }
    public void HandleDiceRolled(int sum)
    {
        StartCoroutine(MovePlayerByDiceAmount(sum));
    }
    public System.Collections.IEnumerator MovePlayerByDiceAmount(int sum)
    {
        // wait for a second
        yield return new WaitForSeconds(2);
        // Move the current player piece by the sum
        Debug.Log($"Moving player {GameManager.Instance.currentTurnPlayerIndex} by {sum} spaces.");
        MovePlayerPiece(GameManager.Instance.currentTurnPlayerIndex, sum);
    }

    private void OnEnable()
    {
        GameManager.OnPlayerMoveRequested += HandlePlayerMoveRequested;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerMoveRequested -= HandlePlayerMoveRequested;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // player index is player id - 1
    public void MovePlayerPiece(int playerIndex, int spaceMovement)
    {
        if (playerIndex > playerPieces.Count)
        {
            Debug.LogError($"Player index {playerIndex} is out of range for player pieces list.");
            return;
        }
        int currentPosition = GameManager.players[playerIndex].currentTileIndex;
        int newPosition = (currentPosition + spaceMovement) % Tiles.Count;
        if(newPosition < currentPosition)
        {
            // Passed Start tile
            GameManager.Instance.UpdatePlayerMoney(playerIndex + 1, 100); // +1 because playerId is index + 1
            Debug.Log($"Player {playerIndex + 1} passed Start and collected $100.");
        }
        GameManager.players[playerIndex].currentTileIndex = newPosition;

        Vector3 targetPosition = Tiles[newPosition].GetPiecePlacementPosition(playerIndex);
        playerPieces[playerIndex].GetComponent<MovingBehavior>().StartCoroutine(
            playerPieces[playerIndex].GetComponent<MovingBehavior>().HopToPosition(
                targetPosition
            )
        );
        playerPieces[playerIndex].transform.position = targetPosition;
        switch (Tiles[newPosition].tileType)
        {
            case TileType.Property:
                Debug.Log($"Player {playerIndex + 1} landed on a Property tile.");
                GameManager.OnPropertyLandedOn?.Invoke(Tiles[newPosition]);
                break;
            case TileType.Trivia:
                Debug.Log($"Player {playerIndex + 1} landed on a Trivia tile.");
                GameManager.OnTriviaLandedOn?.Invoke();
                break;
            case TileType.Event:
                Debug.Log($"Player {playerIndex + 1} landed on an Event tile.");
                GameManager.OnEventLandedOn?.Invoke();
                break;
            case TileType.Start:
                GameManager.Instance.NextTurn();
                break;
            case TileType.Corner:
                GameManager.Instance.NextTurn();
                break;
            default:
                Debug.Log($"Player {playerIndex + 1} landed on a tile of type {Tiles[newPosition].tileType}.");
                break;
        }
    }

    private void HandlePlayerMoveRequested(int playerIndex, int boardSpaceIndex)
    {
        MovePlayerPiece(playerIndex, boardSpaceIndex);
    }
    public static Action<int> OnRequestTileTransform;
    // public Transform RequestTileTransform(int tileIndex)
    // {
    //     OnRequestTileTransform?.Invoke(tileIndex);
    //     //boardSpaceObjects[tileIndex].GetComponent<TileInformation>().GetPiecePosition();
    // }


    void GetTilesAndPieces()
    {
        // Get all Tile components in children of TileObjects
        Tile[] tileArray = TileObjects.GetComponentsInChildren<Tile>();

        // Convert to list and store in the Tiles field
        Tiles = new List<Tile>(tileArray);

        // Get all player pieces in children of PieceObjects
        playerPieces = new List<GameObject>();
        int count = 0;
        foreach (Transform child in PieceObjects.transform)
        {
            count++;
            child.gameObject.SetActive(true);
            playerPieces.Add(child.gameObject);
            if(GameManager.players.Count == count)
            {
                break;
            }
        }
    }
}
