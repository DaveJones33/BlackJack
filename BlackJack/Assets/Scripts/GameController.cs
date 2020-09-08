using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int _dealersFirstCard = -1;

    [SerializeField] CardDeck _dealer = null;
    [SerializeField] GameObject _dealerObject = null;
    [SerializeField] CardDeck _player = null;
    [SerializeField] CardDeck _deck = null;
    [SerializeField] Button _hitButton = null;
    [SerializeField] Button _stayButton = null;
    [SerializeField] Button _playAgain = null;
    [SerializeField] Text _winText = null;

    private int _dealersHand;
    private int _playersHand;

    public void Hit()
    {
        _player.PushCardInfo(_deck.GetCardInfo());

        _playersHand = _player.GetComponent<DeckCreator>()._handValue;
        
        if (_playersHand > 21)
        {
            _hitButton.interactable = false;
            _stayButton.interactable = false;

            StartCoroutine(DealersTurn());
        }
    }

    public void Stay()
    {
        _hitButton.interactable = false;
        _stayButton.interactable = false;

        StartCoroutine(DealersTurn());
    }

    public void PlayAgain()
    {
        _playAgain.interactable = false;
        _winText.enabled = false;

        _player.GetComponent<DeckCreator>().CleanUp();
        _dealer.GetComponent<DeckCreator>().CleanUp();
        _deck.GetComponent<DeckCreator>().CleanUp();
        _deck.CreateDeck();

        _hitButton.interactable = true;
        _stayButton.interactable = true;
        _dealersFirstCard = -1;

        StartGame();
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        for (var i = 0; i < 2; i++)
        {
            _player.PushCardInfo(_deck.GetCardInfo());
            HitDealer();
        }
    }

    private void HitDealer()
    {
        int card = _deck.GetCardInfo();

        if (_dealersFirstCard < 0)
            _dealersFirstCard = card;

        _dealer.PushCardInfo(card);

        if (_dealer.CardCount >= 2)
        {
            DeckCreator pCard = _dealer.GetComponent<DeckCreator>();
            pCard.Toggle(card, true);
            _dealersHand = GetDealerCards();
        }
    }

    IEnumerator DealersTurn()
    {
        _hitButton.interactable = false;
        _stayButton.interactable = false;

        DeckCreator dCard = _dealer.GetComponent<DeckCreator>();
        dCard.Toggle(_dealersFirstCard, true);
        dCard.ShowCards();
        _dealersHand = GetDealerCards();
        yield return new WaitForSeconds(0.5f);

        while (_dealersHand < 17)
        {
            HitDealer();
            yield return new WaitForSeconds(0.5f);
        }

        _dealersHand = GetDealerCards();
        if (_playersHand > 21 || (_dealersHand >= _playersHand && _dealersHand <= 21))
        {
            _winText.text = "You Lost";
            _winText.enabled = true;
        }
        else if (_dealersHand > 21 || (_playersHand <= 21 && _playersHand > _dealersHand))
        {
            _winText.text = "You Win";
            _winText.enabled = true;
        }

        yield return new WaitForSeconds(1f);
        _playAgain.interactable = true;
    }

    private int GetDealerCards()
    {
        List<int> handValue = new List<int>();

        foreach(Transform child in _dealerObject.transform)
        {
            var cardValue = child.GetComponent<PlayingCard>()._cardValue;
            handValue.Add(cardValue);
        }

        int result = handValue.Sum();

        return result;
    }
}
