using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingCard : MonoBehaviour
{
    public int _cardIndex;
    public AnimationCurve _animationCurve;
    public float _duration = 0.5f;
    public int _cardValue;

    [SerializeField] public Sprite cardBack;
    [SerializeField] public CardScriptableObject showCards;

    public void DisplayFace(bool displayCard)
    {
        if (displayCard)
        {
            // Show card
            var showCard = GetComponentInChildren<Image>();
            //showCard.sprite = showCards._cards[_cardIndex];
            showCard.sprite = showCards._cards[_cardIndex]._card;
            _cardValue = showCards._cards[_cardIndex]._value;
        }
        else
        {
            // Show the back
            var showBack = GetComponentInChildren<Image>();
            showBack.sprite = cardBack;
        }
    }

    public void TurnCard(Sprite startImage, Sprite endImage, int cardIndex)
    {
        StopCoroutine(Turn(startImage, endImage, cardIndex));
        StartCoroutine(Turn(startImage, endImage, cardIndex));
    }

    IEnumerator Turn(Sprite startImage, Sprite endImage, int cardIndex)
    {
        var cardDisplay = GetComponentInChildren<Image>();
        cardDisplay.sprite = startImage;

        float time = 0f;
        while (time <= 1f)
        {
            float scale = _animationCurve.Evaluate(time);
            time = time + Time.deltaTime / _duration;

            Vector3 localScale = transform.localScale;
            localScale.x = scale;
            transform.localScale = localScale;

            if (time >= 0.5f)
                cardDisplay.sprite = endImage;

            yield return new WaitForFixedUpdate();
        }

        if (cardIndex == -1)
            DisplayFace(false);
        else
        {
            _cardIndex = cardIndex;
            DisplayFace(true);
        }
    }
}
