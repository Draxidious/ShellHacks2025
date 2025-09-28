using System;
using TMPro;
using UnityEngine;

public class TriviaCardUI : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text answerText;

    private TriviaQuestion _q;
    private Action _onClosed;

    public void Show(TriviaQuestion q, Action onClosed)
    {
        _q = q;
        _onClosed = onClosed;
        questionText.text = q.question;
        answerText.text = "Tap Reveal to see the answer";
        gameObject.SetActive(true);
    }

    public void OnReveal()
    {
        if (_q != null) answerText.text = _q.answer;
    }

    public void OnClose()
    {
        _onClosed?.Invoke();
        Destroy(gameObject);
    }
}
