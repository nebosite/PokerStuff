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

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// General model for displaying hand data
    /// </summary>
    //------------------------------------------------------------------------------------
    public class HandModel : BaseModel
    {
        public CardModel PocketCard1 => GetCardModel(0);
        public CardModel PocketCard2 => GetCardModel(1);

        public CardModel FlopCard1 => GetCardModel(2);
        public CardModel FlopCard2 => GetCardModel(3);
        public CardModel FlopCard3 => GetCardModel(4);

        public CardModel TurnCard => GetCardModel(5);
        public CardModel RiverCard => GetCardModel(6);

        public ulong CardBits => _playerHand.CardBits;

        public CardModel[] Cards => _playerHand.DealtCards.Select(c => GetCardModel(c)).ToArray();

        private Hand _playerHand = new Hand();


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset to a new empty hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Reset()
        {
            _playerHand = new Hand();
            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Create a card model for a card in the hand
        /// </summary>
        //------------------------------------------------------------------------------------
        CardModel GetCardModel(int slot)
        {
            var card = _playerHand?.GetDealtCard(slot);
            if (card == null) return null;
            return GetCardModel(card);
        }
        CardModel GetCardModel(Card card)
        {
            return new CardModel(card);
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add a new card to the hand
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void AddCard(Card card)
        {
            _playerHand.AddCard(card);
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
