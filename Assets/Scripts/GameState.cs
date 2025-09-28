using TMPro;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public GameObject buyPropertyPanel;
    public GameObject payThePricePanel;
    public GameObject rollDicePanel;

    public GameObject gameWinPanel;
    public TMP_Text gameWinText;
    public bool buyProperty = false;
    public bool declineProperty = false;

    public bool gameOver = false;

    private Tile currentTile; // Track the tile the player landed on
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnPropertyLandedOn += PropertyLandedOn;
        GameManager.OnTriviaLandedOn += TriviaLandedOn;
        GameManager.OnEventLandedOn += EventLandedOn;
        GameManager.OnGameWin += HandleGameWin;
    }

    void HandleGameWin(int playerId)
    {
        gameOver = true;
        buyPropertyPanel.SetActive(false);
        rollDicePanel.SetActive(false);
        payThePricePanel.SetActive(false);
        gameWinPanel.SetActive(true);
        gameWinText.text = $"{GameManager.GetPlayer(playerId).playerName} wins the game!";
    }

    public void PropertyLandedOn(Tile tile)
    {
        int playerIndex = GameManager.Instance.currentTurnPlayerIndex;
        if (playerIndex == tile.playerOwnerId - 1)
        {
            GameManager.Instance.NextTurn();
            return;
        }
        if (tile.playerOwnerId != -1)
        {
            Player currentPlayer = GameManager.players[GameManager.Instance.currentTurnPlayerIndex];
            if (tile.tileType != TileType.Property || tile.playerOwnerId == 0 || tile.playerOwnerId == currentPlayer.id)
            {
                Debug.Log("No rent to pay.");
                return;
            }
            Player owner = GameManager.GetPlayer(tile.playerOwnerId);
            Debug.Log($"Property owner ID: {tile.playerOwnerId}");
            if (owner == null)
            {
                Debug.LogError("Property owner not found.");
                return;
            }
            float rentAmount = tile.rentAmount;
            GameManager.Instance.UpdatePlayerMoney(currentPlayer.id, -(int)rentAmount);
            GameManager.Instance.UpdatePlayerMoney(owner.id, (int)rentAmount);
            Debug.Log($"{currentPlayer.playerName} paid ${rentAmount} in rent to {owner.playerName} for landing on {tile.tileName}.");
            if(gameOver) return;
            StartCoroutine(ShowPricePaidPanel());
            return;
        }
        buyPropertyPanel.SetActive(true);
        rollDicePanel.SetActive(false);
        currentTile = tile;
    }

    private System.Collections.IEnumerator ShowPricePaidPanel()
    {
        rollDicePanel.SetActive(false);
        payThePricePanel.SetActive(true);
        yield return new WaitForSeconds(2); // Display for 2 seconds
        payThePricePanel.SetActive(false);
        rollDicePanel.SetActive(true);
        GameManager.Instance.NextTurn();
    }

    public void Update()
    {
        if (buyProperty)
        {
            PropertyBought();
            buyProperty = false;
        }
        else if (declineProperty)
        {
            PropertyDeclined();
            declineProperty = false;
        }
    }

    public void PropertyBought()
    {
        if (GameManager.players[GameManager.Instance.currentTurnPlayerIndex].money < currentTile.propertyCost)
        {
            Debug.Log("Not enough money to buy this property.");
            return;
        }
        buyPropertyPanel.SetActive(false);
        rollDicePanel.SetActive(true);
        currentTile.setOwner(GameManager.Instance.currentTurnPlayerIndex + 1);

        GameManager.Instance.UpdatePlayerMoney(GameManager.Instance.currentTurnPlayerIndex + 1, -currentTile.propertyCost);
        Debug.Log($"{GameManager.players[GameManager.Instance.currentTurnPlayerIndex].playerName} bought {currentTile.tileName} for ${currentTile.propertyCost}.");
        GameManager.Instance.NextTurn();
    }

    public void PropertyDeclined()
    {
        buyPropertyPanel.SetActive(false);
        rollDicePanel.SetActive(true);
        Debug.Log($"{GameManager.players[GameManager.Instance.currentTurnPlayerIndex].playerName} declined to buy {currentTile.tileName}.");
        GameManager.Instance.NextTurn();
    }
    public void TriviaLandedOn()
    {
        Debug.Log("Trivia Landed On - TileManager");
    }
    public void EventLandedOn()
    {
        Debug.Log("Event Landed On - TileManager");
    }
}
