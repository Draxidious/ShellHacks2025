using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string playerName = "Player";
    public float money = 0;
    // convenience
    public override string ToString()
    {
        return $"[{id}] {playerName} (H, Score: {money})";
    }
}
