using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BlackJackGameController : MonoBehaviour
{
    [SerializeField] CardDeck _deck = null;
    [SerializeField] CardDeck _player = null;
    [SerializeField] CardDeck _dealer = null;
    [SerializeField] Button _hitButton = null;
    [SerializeField] Button _stayButton = null;
    [SerializeField] Button _playAgain = null;
    [SerializeField] TextMeshProUGUI _handValue = null;
    [SerializeField] TextMeshProUGUI _dealerValue = null;
    [SerializeField] GameObject _winnerPanel = null;
    [SerializeField] GameObject _loserPanel = null;
    [SerializeField] GameObject _drawPanel = null;

    private int _dealersFirstCard = -1;
    private int _playersHand;
    private int _dealersHand;

    // Deal the Player another card
    public void Hit()
    {
        _player.PushCardInfo(_deck.GetCardInfo());
        _playersHand = _player.GetComponent<DeckCreator>().ReturnHandValue();
        _handValue.text = _player.GetComponent<DeckCreator>().ReturnHandValue().ToString();

        // if the Players hand is greater than 21 they bust and ask to play again
        if (_playersHand > 21)
        {
            _hitButton.interactable = false;
            _stayButton.interactable = false;

            BustGame();
        }
    }

    // The Player telling the dealer that they don't want another card
    public void Stay()
    {
        _hitButton.interactable = false;
        _stayButton.interactable = false;

        // Start the dealers turn
        StartCoroutine(DealersTurn());
    }

    // Tell the Dealer that the Player wants to start another game
    public void PlayAgain()
    {
        _playAgain.interactable = false;
        _hitButton.interactable = true;
        _stayButton.interactable = true;

        _player.GetComponent<DeckCreator>().CleanUp();
        _dealer.GetComponent<DeckCreator>().CleanUp();
        _deck.GetComponent<DeckCreator>().CleanUp();
        _deck.CreateDeck();

        _dealersFirstCard = -1;
        StartGame();
    }

    private void Start()
    {
        StartGame();
    }

    // Things that should happen when the game starts
    private void StartGame()
    {
        _handValue.text = "";
        _dealerValue.text = "";

        for (var i = 0; i < 2; i++)
        {
            _player.PushCardInfo(_deck.GetCardInfo());
            HitDealer();
        }

        // Set the initial hand value when the Player has both cards
        _playersHand = _player.GetComponent<DeckCreator>().ReturnHandValue();
        _handValue.text = _playersHand.ToString();
        _dealersHand = _dealer.GetComponent<DeckCreator>().ReturnHandValue();

        // If the Player has 21 they automatically win
        if (_playersHand == 21)
        {
            _hitButton.interactable = false;
            _stayButton.interactable = false;

            // Show the win display and activate the play again button
            GameWon();
        }

        // TODO: I'm thinking here is where I need to reevaluate the players hand for a split if its available
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
        }
    }

    // The game was won by the Player show the win display
    private void GameWon()
    {
        _winnerPanel.SetActive(true);
        _winnerPanel.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InOutBounce)
            .OnComplete(() =>
            {
                _winnerPanel.transform.DOScale(0.0f, 1.0f).SetEase(Ease.Linear)
                .OnComplete(() => {
                    _winnerPanel.SetActive(false);
                    _playAgain.interactable = true;
                });
            });
    }

    // If the game goes bust lets show the lost game screen and enable the play again button
    private void BustGame()
    {
        _loserPanel.SetActive(true);
        _loserPanel.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InOutBounce)
            .OnComplete(() => 
            {
                _loserPanel.transform.DOScale(0.0f, 1.0f).SetEase(Ease.Linear)
                    .OnComplete(() => {
                        _loserPanel.SetActive(false);
                        _playAgain.interactable = true; });
            });
    }

    // The Dealers turn after the Player hits the stay button
    IEnumerator DealersTurn()
    {
        _hitButton.interactable = false;
        _stayButton.interactable = false;

        var dCard = _dealer.GetComponent<DeckCreator>();
        dCard.Toggle(_dealersFirstCard, true);
        dCard.ShowCards();
        _dealersHand = _dealer.GetComponent<DeckCreator>().ReturnHandValue();
        _dealerValue.text = _dealersHand.ToString();
        
        yield return new WaitForSeconds(0.5f);

        while (_dealersHand < 17)
        {
            HitDealer();
            _dealersHand = _dealer.GetComponent<DeckCreator>().ReturnHandValue();
            _dealerValue.text = _dealersHand.ToString();
            
            yield return new WaitForSeconds(0.5f);
        }

        // If the Dealer and the Player have the same total the its a Push
        if (_dealersHand == _playersHand)
        {
            _drawPanel.SetActive(true);
            _drawPanel.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InOutBounce)
                .OnComplete(() =>
                {
                    _drawPanel.transform.DOScale(0.0f, 1.0f).SetEase(Ease.Linear)
                    .OnComplete(() => {
                        _drawPanel.SetActive(false);
                    });
                });
        }
        // If the Dealer has a hand of 21 or is greater than the Player then the Player lost
        else if (_dealersHand <= 21 && _dealersHand > _playersHand)
        {
            _loserPanel.SetActive(true);
            _loserPanel.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InOutBounce)
                .OnComplete(() =>
                {
                    _loserPanel.transform.DOScale(0.0f, 1.0f).SetEase(Ease.Linear)
                    .OnComplete(() => {
                        _loserPanel.SetActive(false);
                    });
                });
        }
        // If the Dealer has a hand that bust or is less than the Players hand the Player wins
        else if (_dealersHand > 21 || (_playersHand <= 21 && _playersHand > _dealersHand))
        {
            _winnerPanel.SetActive(true);
            _winnerPanel.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InOutBounce)
                .OnComplete(() =>
                {
                    _winnerPanel.transform.DOScale(0.0f, 1.0f).SetEase(Ease.Linear)
                    .OnComplete(() => {
                        _winnerPanel.SetActive(false);
                    });
                });
        }

        yield return new WaitForSeconds(1f);
        _playAgain.interactable = true;
    }
}
