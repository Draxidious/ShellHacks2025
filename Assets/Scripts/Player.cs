using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id = 0;
    public string playerName = "Player";
    public string profession = "Unemployed";
    public float money = 0;
    public bool isBankrupt = false;

    public List<string> events;

    public int currentTileIndex = 0;
    // convenience
    public override string ToString()
    {
        return $"{playerName} (H, Score: {money})";
    }
}
