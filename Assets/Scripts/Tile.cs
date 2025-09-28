using System.Collections.Generic;
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

    public string tileName = "Property";
    public int propertyCost = 100;
    public int rentAmount = 100;

    public int playerOwnerId = -1; // -1 means no owner

    public Vector3 GetPiecePlacementPosition(int playerIndex)
    {
        Vector3 piecePlacement = piecePlacements[playerIndex].position;
        return piecePlacement;
    }
}
