using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public List<Transform> piecePlacements = new List<Transform>();
    public Vector3 GetPiecePlacementPosition(int playerIndex)
    {
        Vector3 piecePlacement = piecePlacements[playerIndex].position;
        return piecePlacement;
        
    }
}
