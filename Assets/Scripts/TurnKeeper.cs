using UnityEngine;
using TMPro;
public class TurnKeeper : MonoBehaviour
{
    public TMP_Text turnText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnTurnChanged += TurnChanged;
    }

    void TurnChanged()
    {
        Debug.Log("Turn Changed");
        turnText.text = $"It is {GameManager.players[GameManager.Instance.currentTurnPlayerIndex].playerName}'s turn.";
    }
}
