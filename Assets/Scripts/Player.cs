using UnityEngine;

public class Player : MonoBehaviour
{
    public int id = 0;
    public string playerName = "Player";
    public string profession = "Unemployed";
    public float money = 0;

    public int currentTileIndex = 0;
    // convenience
    public override string ToString()
    {
        return $"{playerName} (H, Score: {money})";
    }
}
