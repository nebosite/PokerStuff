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
        public Card PocketCard1 => _playerHand?.GetDealtCard(0);
        public Card PocketCard2 => _playerHand?.GetDealtCard(1);

        public Card FlopCard1 => _playerHand?.GetDealtCard(2);
        public Card FlopCard2 => _playerHand?.GetDealtCard(3);
        public Card FlopCard3 => _playerHand?.GetDealtCard(4);

        public Card TurnCard => _playerHand?.GetDealtCard(5);
        public Card RiverCard => _playerHand?.GetDealtCard(6);

        public ulong CardBits => _playerHand.CardBits;

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
        /// Figure the odds for this hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public OddsResults CalculateOdds(Deck deck, int playerCount)
        {
            return OddsCalculator.Calculate(deck, _playerHand, playerCount, TimeSpan.FromMilliseconds(100));
        }
    }
}
