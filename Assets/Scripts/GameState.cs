using UnityEngine;

public class GameState : MonoBehaviour
{
    public GameObject buyPropertyPanel;
    public GameObject payThePricePanel;
    public GameObject rollDicePanel;

    public GameObject triviaPanel;
    public GameObject eventPanel;

    public bool buyProperty = false;
    public bool declineProperty = false;

    private Tile currentTile; // Track the tile the player landed on
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.OnPropertyLandedOn += PropertyLandedOn;
        GameManager.OnTriviaLandedOn += TriviaLandedOn;
        GameManager.OnEventLandedOn += EventLandedOn;
    }

    public void PropertyLandedOn(Tile tile)
    {
        if (tile.playerOwnerId != -1)
        {
            Player currentPlayer = GameManager.players[GameManager.Instance.currentTurnPlayerIndex];
            if (currentTile.tileType != TileType.Property || currentTile.playerOwnerId == 0 || currentTile.playerOwnerId == currentPlayer.id)
            {
                Debug.Log("No rent to pay.");
                return;
            }
            Player owner = GameManager.GetPlayer(currentTile.playerOwnerId);
            if (owner == null)
            {
                Debug.LogError("Property owner not found.");
                return;
            }
            float rentAmount = currentTile.rentAmount;
            if (currentPlayer.money < rentAmount)
            {
                Debug.Log($"{currentPlayer.playerName} does not have enough money to pay rent of ${rentAmount} to {owner.playerName}.");
                // Handle bankruptcy or other logic here
                return;
            }
            GameManager.Instance.UpdatePlayerMoney(currentPlayer.id, - (int)rentAmount);
            GameManager.Instance.UpdatePlayerMoney(owner.id, (int)rentAmount);
            Debug.Log($"{currentPlayer.playerName} paid ${rentAmount} in rent to {owner.playerName} for landing on {currentTile.tileName}.");
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
        currentTile.setOwner(GameManager.Instance.currentTurnPlayerIndex);

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
