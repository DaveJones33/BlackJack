using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    private List<int> _cards;
    public bool _isGameDeck;

    void Awake()
    {
        _cards = new List<int>();
        if (_isGameDeck)
        {
            CreateDeck();
        }
    }

    public bool HasCards
    {
        get { return _cards != null && _cards.Count > 0; }
    }

    public event EventManager.CardEventHandler CardRemoved;
    public event EventManager.CardEventHandler CardAdded;

    public int CardCount
    {
        get
        {
            if (_cards == null)
                return 0;
            else
                return _cards.Count;
        }
    }

    public void CreateDeck()
    {
        _cards.Clear();

        for (var i = 0; i < 52; i++)
        {
            _cards.Add(i);
        }

        _cards = _cards.OrderBy(i => Guid.NewGuid()).ToList();
    }

    public int GetCardInfo()
    {
        var temp = _cards[0];
        _cards.RemoveAt(0);

        CardRemoved?.Invoke(temp);

        return temp;
    }

    public void PushCardInfo(int card)
    {
        _cards.Add(card);

        CardAdded?.Invoke(card);
    }

    public void Reset()
    {
        _cards.Clear();
    }

    public IEnumerable<int> GetCards()
    {
        foreach (int i in _cards)
        {
            yield return i;
        }
    }
}
