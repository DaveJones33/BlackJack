using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public GameObject _card { get; private set; }
    public bool _isFaceUp { get; set; }

    public CardView(GameObject card)
    {
        _card = card;
        _isFaceUp = false;
    }
}
