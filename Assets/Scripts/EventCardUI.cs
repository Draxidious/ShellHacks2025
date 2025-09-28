using System;
using TMPro;
using UnityEngine;

public class EventCardUI : MonoBehaviour
{
    public TMP_Text descriptionText;

    private CommunityChestCard _card;
    private Action _onResolved;

    public void Show(CommunityChestCard card, Action onResolved)
    {
        _card = card;
        _onResolved = onResolved;
        descriptionText.text = card.description;
        gameObject.SetActive(true);
    }

    public void OnConfirm()
    {
        CardResolver.Resolve(_card);
        _onResolved?.Invoke();
        Destroy(gameObject);
    }

    public void OnClose()
    {
        _onResolved?.Invoke();
        Destroy(gameObject);
    }
}
