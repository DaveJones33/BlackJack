using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void CardEventHandler(int cardIndex);

    public int _cardIndex { get; private set; }

    public void CardRemoved(int index)
    {
        _cardIndex = index;
    }
}
