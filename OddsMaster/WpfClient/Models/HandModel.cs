using Newtonsoft.Json;
using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// General model for displaying hand data
    /// </summary>
    //------------------------------------------------------------------------------------
    public class HandModel : BaseModel
    {
        public CardModel PocketCard1 => GetCard(0);
        public CardModel PocketCard2 => GetCard(1);

        public CardModel FlopCard1 => GetCard(2);
        public CardModel FlopCard2  => GetCard(3);
        public CardModel FlopCard3 => GetCard(4);

        public CardModel TurnCard => GetCard(5);
        public CardModel RiverCard => GetCard(6);

        public ulong CardBits => _playerHand.CardBits;

        public List<CardModel> Cards { get; private set; } = new List<CardModel>();

        private Hand _playerHand = new Hand();

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public HandModel()
        {

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset to a new empty hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Reset()
        {
            Cards.Clear();
            _playerHand = new Hand();
            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Get a card from the hand
        /// </summary>
        //------------------------------------------------------------------------------------
        CardModel GetCard(int slot)
        {
            if (Cards.Count > slot) return Cards[slot];
            return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add a new card to the hand
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void AddCard(Card card)
        {
            _playerHand.AddCard(card);
            var newCard = new CardModel(card);
            Cards.Add(newCard);
            switch (_playerHand.DealtCards.Count)
            {
                case 1: NotifyPropertyChanged(nameof(PocketCard1)); break;
                case 2: NotifyPropertyChanged(nameof(PocketCard2)); break;
                case 3: NotifyPropertyChanged(nameof(FlopCard1)); break;
                case 4: NotifyPropertyChanged(nameof(FlopCard2)); break;
                case 5: NotifyPropertyChanged(nameof(FlopCard3)); break;
                case 6: NotifyPropertyChanged(nameof(TurnCard)); break;
                case 7: NotifyPropertyChanged(nameof(RiverCard)); break;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Swap one of the cards in our hand with another card in the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void Swap(CardModel targetCard, Card cardInDeck)
        {
            _playerHand.Swap(targetCard.Card, cardInDeck);
            Cards.Clear();
            foreach(var card in _playerHand.DealtCards)
            {
                Cards.Add(new CardModel(card));
            }
            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Figure the odds for this hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public OddsResults CalculateOdds(Deck deck, int playerCount)
        {
            return OddsCalculator.Calculate(deck, _playerHand, playerCount, TimeSpan.FromMilliseconds(100));
        }
    }
}
