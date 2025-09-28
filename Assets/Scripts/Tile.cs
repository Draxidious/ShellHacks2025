using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum TileType
{
    Start,
    Property,
    Trivia,
    Event,
}
public class Tile : MonoBehaviour
{
    public List<Transform> piecePlacements = new List<Transform>();
    public TileType tileType = TileType.Property;

    public TMP_Text ownerText;

    public string tileName = "Property";
    public int propertyCost = 100;
    public int rentAmount = 100;
    public int playerOwnerId = -1; // -1 means no owner

    public Vector3 GetPiecePlacementPosition(int playerIndex)
    {
        Vector3 piecePlacement = piecePlacements[playerIndex].position;
        return piecePlacement;
    }

    public void setOwner(int playerId)
    {
        if (playerId < 0)
        {
            playerOwnerId = -1;
            ownerText.text = "No Current Owner";
            return;
        }
        playerOwnerId = playerId;
        ownerText.text = $"Owner: Player {playerId}";
    }
}
