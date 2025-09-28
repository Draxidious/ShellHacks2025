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
        GameManager.OnProfessionUpdated += ProfessionUpdated;
    }

    void PlayerAdded(Player player)
    {
        int playerIndex = player.id - 1;
        if (playerIndex == tileIndex)
        {
            Components.SetActive(true);
            playerName.text = player.playerName;
            money.text = $"Money: ${player.money}";
            profession.text = $"Career: {player.profession}";
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

    void ProfessionUpdated(int playerId, string newProfession)
    {
        int playerIndex = playerId - 1;
        if (playerIndex == tileIndex)
        {
            Debug.Log($"Updating profession for player {playerId} to {newProfession}");
            Player player = GameManager.players[playerIndex];
            if (player == null) return;
            player.profession = newProfession;
            profession.text = $"Career: {newProfession}";
        }
    }
    
}
