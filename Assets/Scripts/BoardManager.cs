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

    public void MovePlayerPiece(int playerIndex, int spaceMovement)
    {
        if (playerIndex > playerPieces.Count)
        {
            Debug.LogError($"Player index {playerIndex} is out of range for player pieces list.");
            return;
        }
        Debug.Log($"tiels count: {Tiles.Count}");
        int newPosition = (playerIndex + spaceMovement) % Tiles.Count;
        playerPieces[playerIndex].transform.position = Tiles[newPosition].piecePlacement.position;
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
