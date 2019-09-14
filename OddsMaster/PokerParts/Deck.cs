using PokerParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParts
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
        Card[] _cards = new Card[52];

        public Card[] AllCards => _cards;
        ulong _drawnCards = 0;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor - creats a fresh, ordered deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public Deck()
        {
            var ranks = "234567890jqka";
            var suits = "CDHS";
            int index = 0;
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 13; j++)
                {
                    _cards[index++] = new Card(ranks[j], suits[i]);
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
            for (int i = DrawSpot; i < _cards.Length - 1; i++)
            {
                var remainingCount = _cards.Length - i - 1;
                var swapIndex = _random.Next(remainingCount) + i + 1;
                var temp = _cards[swapIndex];
                _cards[swapIndex] = _cards[i];
                _cards[i] = temp;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate all possible pairs of cards from the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public IEnumerable<Card[]> GetAllAvailablePairs()
        {
            for (int i = DrawSpot; i < _cards.Length; i++)
            {
                for (int j = i + 1; j < _cards.Length; j++)
                {
                    yield return new Card[] { AllCards[i], AllCards[j] };
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Swap the two cards in the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Swap(Card targetCard, Card sourceCard)
        {
            var targetIndex = IndexOf(targetCard);
            var sourceIndex = IndexOf(sourceCard);

            var temp = _cards[targetIndex];
            _cards[targetIndex] = _cards[sourceIndex];
            _cards[sourceIndex] = temp;
            _drawnCards &= ~(targetCard.Bit);
            _drawnCards |= sourceCard.Bit;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Get the index of a card
        /// </summary>
        //------------------------------------------------------------------------------------
        private int IndexOf(Card card)
        {
            for(int i = 0; i < _cards.Length; i++)
            {
                if (_cards[i].Bit == card.Bit) return i;
            }
            throw new ApplicationException("The deck is missing a card.");
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Return the card object from the deck that matches the input card
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card FindCard(Card card)
        {
            return _cards[IndexOf(card)];
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draw the top card from the deck (advances the DrawSpot)
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card Draw()
        {
            if (DrawSpot >= _cards.Length) throw new ApplicationException("Tried to draw from an empty deck.");
            var drawnCard = _cards[DrawSpot++];
            _drawnCards |= drawnCard.Bit;
            return drawnCard;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Draw the exact card from anywhere in the remaining cards
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card Draw(Card card)
        {
            return Draw(card.Rank, card.Suit);
        }

        public Card Draw(Rank rank, Suit suit)
        {
            for(int i = DrawSpot; i < _cards.Length; i++)
            {
                if(_cards[i].Rank == rank && _cards[i].Suit == suit)
                {
                    var temp = _cards[i];
                    _cards[i] = _cards[DrawSpot];
                    _cards[DrawSpot] = temp;
                    return Draw();
                }
            }
            return null;
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
            _drawnCards = 0;
            for(int i = 0; i < newSpot; i++)
            {
                _drawnCards |= _cards[i].Bit;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// return true if cards are still int the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool CanDraw(params Card[] cards)
        {
            for(int j = 0; j < cards.Length; j++)
            {
                if ((_drawnCards & cards[j].Bit) > 0) return false;
            }
            return true;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Return a string that identifies a pair of cards.
        /// e.g.:  K9s, AJo
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetPairType(Card[] pair)
        {
            // high card is first
            if (pair[0].Rank < pair[1].Rank)
            {
                var temp = pair[0];
                pair[0] = pair[1];
                pair[1] = temp;
            }

            var letter1 = Hand.GetRankChar(pair[0].Rank);
            var letter2 = Hand.GetRankChar(pair[1].Rank);
            var suitLetter = "";
            if (pair[0].Rank != pair[1].Rank)
            {
                suitLetter = pair[0].Suit == pair[1].Suit ? "s" : "o";
            }


            return $"{letter1}{letter2}{suitLetter}";
        }

    }
}
