using UnityEngine;
using TMPro;

public class PlayerTile : MonoBehaviour
{
    public GameObject Components;
    public TMP_Text playerName;
    public TMP_Text money;
    public TMP_Text profession;

    public int tileIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnPlayerAdded += PlayerAdded;
        GameManager.OnMoneyUpdated += MoneyUpdated;
    }

    void PlayerAdded(Player player)
    {
        int playerIndex = player.id - 1;
        if (playerIndex == tileIndex)
        {
            Components.SetActive(true);
            playerName.text = player.playerName;
            money.text = $"Money: ${player.money}";
            profession.text = $"Profession: {player.profession}";
        }
    }

    void MoneyUpdated(int playerId, int amount)
    {
        int playerIndex = playerId - 1;
        if (playerIndex == tileIndex)
        {
            Player player = GameManager.players[playerIndex];
            if (player == null) return;
            if (player.money < 0)
            {
                Components.SetActive(false);
                player.isBankrupt = true;
                return;
            }
            money.text = $"Money: ${player.money}";
        }
    }
    
}
