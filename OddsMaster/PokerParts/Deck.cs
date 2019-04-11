using PokerParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{

    //------------------------------------------------------------------------------------
    /// <summary>
    /// Deck
    /// </summary>
    //------------------------------------------------------------------------------------
    public class Deck
    {
        /// <summary>
        /// The current location of the draw
        /// </summary>
        public int DrawSpot { get; private set; } = 0;

        Random _random = new Random();
        List<Card> _cards = new List<Card>();

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor - creats a fresh, ordered deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public Deck()
        {
            var ranks = "234567890jqka";
            var suits = "CDHS";
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 13; j++)
                {
                    _cards.Add(new Card(ranks[j], suits[i]));
                }
            }
            DrawSpot = 0;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Shuffle the undrawn portion of the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Shuffle()
        {
            for (int i = DrawSpot; i < _cards.Count - 1; i++)
            {
                var remainingCount = _cards.Count - i - 1;
                var swapIndex = _random.Next(remainingCount) + i + 1;
                var temp = _cards[swapIndex];
                _cards[swapIndex] = _cards[i];
                _cards[i] = temp;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draw the top card from the deck (advances the DrawSpot)
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card Draw()
        {
            if (_cards.Count == 0) throw new ApplicationException("Tried to draw from an empty deck.");
            return _cards[DrawSpot++];
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Put card back onto the deck in thier original order.  Specifying newspot will
        /// allow you to put the deck just partially
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Reset(int newSpot = 0)
        {
            DrawSpot = newSpot;
        }
    }
}
