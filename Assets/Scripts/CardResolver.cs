using UnityEngine;

public static class CardResolver
{
    public static void Resolve(CommunityChestCard card)
    {
        if (card == null) return;
        var gm = GameManager.Instance;
        var currentPlayer = GameManager.players[gm.currentTurnPlayerIndex];

        switch (card.action)
        {
            case CardAction.CollectFromAllPlayers:
                foreach (var p in GameManager.players)
                {
                    if (p.id == currentPlayer.id || p.isBankrupt) continue;
                    gm.UpdatePlayerMoney(p.id, -card.amount);
                }
                gm.UpdatePlayerMoney(currentPlayer.id, +card.amount * (GameManager.players.Count - 1));
                break;

            case CardAction.CollectFromBank:
                gm.UpdatePlayerMoney(currentPlayer.id, +card.amount);
                break;

            case CardAction.PayFine:
                gm.UpdatePlayerMoney(currentPlayer.id, -card.amount);
                break;

            case CardAction.AdvanceToNextProperty:
                AdvanceToNextProperty();
                break;
        }
    }

    private static void AdvanceToNextProperty()
    {
        var bm = BoardManager.Instance;
        var gm = GameManager.Instance;
        int playerIndex = gm.currentTurnPlayerIndex;
        int start = GameManager.players[playerIndex].currentTileIndex;

        for (int step = 1; step <= bm.Tiles.Count; step++)
        {
            int idx = (start + step) % bm.Tiles.Count;
            if (bm.Tiles[idx].tileType == TileType.Property)
            {
                bm.MovePlayerPiece(playerIndex, step);
                return;
            }
        }
    }
}
