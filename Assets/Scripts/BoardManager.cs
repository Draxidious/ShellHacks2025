using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class BoardManager : MonoBehaviour
{
    // Singleton instance for easy global access
    public static BoardManager Instance { get; private set; }

    #region Board variables
    public List<GameObject> boardSpaceObjects;

    public List<Transform> boardSpacePositions;

    #endregion

    #region Player variables
    public List<GameObject> playerPieces;

    #endregion


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetBoardSpacePositions();
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
        if (playerIndex < playerPieces.Count &&  playerIndex + spaceMovement < boardSpacePositions.Count)
        {
            playerPieces[playerIndex].transform.position = boardSpacePositions[playerIndex + spaceMovement].position;
        }
        else
        {
            playerPieces[playerIndex].transform.position = boardSpacePositions[playerIndex + spaceMovement - boardSpacePositions.Count].position;
        }
    }

    private void HandlePlayerMoveRequested(int playerIndex, int boardSpaceIndex)
    {
        MovePlayerPiece(playerIndex, boardSpaceIndex);
    }
    public static Action<int> OnRequestTileTransform;
    public Transform RequestTileTransform(int tileIndex)
    {
        OnRequestTileTransform?.Invoke(tileIndex);
        //boardSpaceObjects[tileIndex].GetComponent<TileInformation>().GetPiecePosition();
    }

    
    void GetBoardSpacePositions()
    {
        boardSpacePositions = new List<Transform>();

        foreach (GameObject space in boardSpaceObjects)
        {
            boardSpacePositions.Add(space.transform);
        }
    }
}
