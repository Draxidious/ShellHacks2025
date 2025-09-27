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
        Debug.Log("BoardManager Start");
        GetTilesAndPieces();
        for (int i = 0; i < playerPieces.Count; i++)
        {
            MovePlayerPiece(i, 0);
        }
        GameManager.OnPlayerAdded += AddPlayerPiece;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddPlayerPiece(Player player)
    {
        int playerIndex = player.id - 1;
        playerPieces[playerIndex].transform.position = Tiles[0].GetPiecePlacementPosition(playerIndex);
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
        GameManager.players[playerIndex].currentTileIndex = newPosition;

        Vector3 targetPosition = Tiles[newPosition].GetPiecePlacementPosition(playerIndex);
        playerPieces[playerIndex].GetComponent<MovingBehavior>().StartCoroutine(
            playerPieces[playerIndex].GetComponent<MovingBehavior>().HopToPosition(
                targetPosition
            )
        );
        playerPieces[playerIndex].transform.position = targetPosition;
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
        foreach (Transform child in PieceObjects.transform)
        {
            playerPieces.Add(child.gameObject);
        }
    }
}
