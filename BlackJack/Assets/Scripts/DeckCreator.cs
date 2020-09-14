using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckCreator : MonoBehaviour
{
    private CardDeck _deck;
    private Dictionary<int, CardView> _fetchedCards;

    public int _handValue = 0;
    public float _cardOffset = 0.5f;
    public bool _faceUp = false;
    public bool _reverseLayerOrder = false;
    public Vector3 _start;
    [SerializeField] GameObject _playingCard = null;
    public List<PlayingCard> _hand = new List<PlayingCard>();


    public void Toggle (int card, bool isFaceUp)
    {
        _fetchedCards[card]._isFaceUp = isFaceUp;
    }

    public void CleanUp()
    {
        _deck.Reset();
        _handValue = 0;

        foreach (CardView view in _fetchedCards.Values)
        {
            Destroy(view._card);
        }

        _fetchedCards.Clear();
    }

    private void Awake()
    {
        _fetchedCards = new Dictionary<int, CardView>();
        _deck = GetComponent<CardDeck>();
        ShowCards();

        _deck.CardRemoved += DeckCardRemoved;
        _deck.CardAdded += DeckCardAdded;
    }

    private void DeckCardRemoved(int cardIndex)
    {
        if (_fetchedCards.ContainsKey(cardIndex))
        {
            Destroy(_fetchedCards[cardIndex]);
            _fetchedCards.Remove(cardIndex);
        }
    }

    private void DeckCardAdded(int index)
    {
        float cardOff = _cardOffset * _deck.CardCount;
        var temp = _start + new Vector3(cardOff, 0f);
        AddCard(temp, index, _deck.CardCount);
    }

    private void Update()
    {
        ShowCards();
    }

    public void ShowCards()
    {
        var cardCount = 0;

        if (_deck.HasCards)
        {
            foreach (int i in _deck.GetCards())
            {
                float cardOff = _cardOffset * cardCount;
                var temp = _start + new Vector3(cardOff, 0f);
                AddCard(temp, i, cardCount);
                cardCount++;
            }
        }
    }

    private void AddCard(Vector3 position, int cardIndex, int positionIndex)
    {
        if (_fetchedCards.ContainsKey(cardIndex))
        {
            if (!_faceUp)
            {
                PlayingCard dealCard = _fetchedCards[cardIndex]._card.GetComponent<PlayingCard>();
                dealCard.DisplayFace(_fetchedCards[cardIndex]._isFaceUp);
            }
            return;
        }

        var card = Instantiate(_playingCard, transform.position, transform.rotation, transform);
        card.transform.position = position;

        PlayingCard playingCard = card.GetComponent<PlayingCard>();
        _hand.Add(playingCard);
        playingCard._cardIndex = cardIndex;
        playingCard.DisplayFace(_faceUp);
        _handValue = HandValue(playingCard._cardValue);
        _fetchedCards.Add(cardIndex,new CardView(card));
    }

    public int ReturnHandValue()
    {        
        return _handValue;
    }

    private int HandValue(int value)
    {
        var currentValue = value;
        int aces = 0;

        if (currentValue != 1)
            _handValue = _handValue + currentValue;
        else
            aces++;

        for (var i = 0; i < aces; i++)
        {
            if (_handValue + 11 <= 21)
                _handValue = _handValue + 11;
            else
                _handValue = _handValue + 1;
        }

        return _handValue;
    }
}
