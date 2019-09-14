using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows.Media;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Common functionality for a model that deals with a single hand
    /// </summary>
    //------------------------------------------------------------------------------------
    abstract class BaseGameAnalysisModel : BaseModel
    {
        int _playerCount = 5;
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value;
                CalculateOdds();
                NotifyPropertyChanged(nameof(PlayerCount));
                HandleHandChanged();
            }
        }

        protected Deck _deck;

        private HandModel _theHand = new HandModel();
        public HandModel PlayerHand
        {
            get => _theHand;
            set
            {
                _theHand = value;
                NotifyPropertyChanged(nameof(PlayerHand));
            }
        }

        private OddsResults _odds;
        public OddsResults Odds
        {
            get => _odds;
            set
            {
                _odds = value;
                NotifyPropertyChanged(nameof(Odds));
            }
        }

        public bool CanDealNext => PlayerHand.RiverCard == null;

        public CardModel[] PickableCards => 
            new Deck()
            .AllCards
            .Select(c=> new CardModel(c, _deck.CanDraw(c)))
            .ToArray();
            
        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public BaseGameAnalysisModel()
        {
            _deck = new Deck();
            Reset();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Replace the target card with the specified card
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ReplaceCard(CardModel targetCard, CardModel card)
        {
            var cardInDeck = _deck.FindCard(card.Card);
            _deck.Swap(targetCard.Card, cardInDeck);
            PlayerHand.Swap(targetCard, cardInDeck);
            HandleHandChanged();
            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Do what you gotta do if the hand changes
        /// </summary>
        //------------------------------------------------------------------------------------
        public abstract void HandleHandChanged();

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset this model and draw some cards
        /// </summary>
        //------------------------------------------------------------------------------------
        virtual public void Reset()
        {
            _deck.Reset();
            _deck.Shuffle();
            PlayerHand.Reset();
            PlayerHand.AddCard(_deck.Draw());
            PlayerHand.AddCard(_deck.Draw());
            NotifyPropertyChanged(nameof(CanDealNext));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deal the next cards
        /// </summary>
        //------------------------------------------------------------------------------------
        virtual public void DealNext()
        {
            if (PlayerHand.FlopCard1 == null)
            {
                PlayerHand.AddCard(_deck.Draw());
                PlayerHand.AddCard(_deck.Draw());
                PlayerHand.AddCard(_deck.Draw());
            }
            else if (PlayerHand.TurnCard == null)
            {
                PlayerHand.AddCard(_deck.Draw());
            }
            else if (PlayerHand.RiverCard == null)
            {
                PlayerHand.AddCard(_deck.Draw());
            }
            else
            {
                Debug.WriteLine("Whoops, this should not happen in DealNext()");
            }

            NotifyPropertyChanged(nameof(CanDealNext));
            NotifyPropertyChanged(nameof(PickableCards));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deal the next cards
        /// </summary>
        //------------------------------------------------------------------------------------
        protected void CalculateOdds()
        {
            Odds = PlayerHand.CalculateOdds(_deck, PlayerCount);
        }
    }
}
