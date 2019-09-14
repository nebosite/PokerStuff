using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParts
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// HandType - Standard Poker value types in order lowest value to highest
    /// </summary>
    //------------------------------------------------------------------------------------
    public enum HandType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a poker hand up to seven cards - Only the best five card 
    /// combination is used to get the value
    /// </summary>
    //------------------------------------------------------------------------------------
    public class Hand
    {
        /// <summary>
        /// The cards dealt to this hand in the order they were added
        /// </summary>
        public List<Card> DealtCards { get; set; } = new List<Card>(7);

        /// <summary>
        /// For marking when a hand is folded
        /// </summary>
        public bool IsFolded { get; set; }

        /// <summary>
        /// The Raw value of this hand - only best five cards are considered
        /// </summary>
        public HandType Value
        {
            get
            {
                Evaluate();
                return _value;
            }
        }

        /// <summary>
        /// The high card of the hand based on the value.  
        /// e.g.:  A pair of 3's with an Ace kicker will show "3" as high card
        /// </summary>
        public Rank HighCard
        {
            get
            {
                Evaluate();
                return _highCards.Count == 0 ? Rank.None : _highCards[0];
            }
        }

        public string PocketId
        {
            get
            {
                if (DealtCards.Count < 2) return "**";
                var c1 = GetRankChar(DealtCards[0].Rank);
                var c2 = GetRankChar(DealtCards[1].Rank);
                var c3 = DealtCards[0].Rank == DealtCards[1].Rank ? "" 
                    : DealtCards[0].Suit == DealtCards[1].Suit ? "s" : "o";
                return $"{c1}{c2}{c3}";
            }
        }
     
        int[] _suitBits = new int[4];
        int[] _suitCounts = new int[4];
        int _rankBits;
        HandType _value;
        List<Rank> _highCards = new List<Rank>(5);
        bool _evaluated = false;
        public ulong CardBits { get; private set; }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public Hand(string[] cards = null)
        {
            if (cards == null) return;
            foreach (var card in cards) AddCard(new Card(card));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Get a printable character for a rank
        /// </summary>
        //------------------------------------------------------------------------------------
        public static char GetRankChar(Rank rank)
        {
            return rank == Rank._10 ? 'T' : rank.ToString().Trim('_')[0];
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Get a card that has been dealt to the player
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card GetDealtCard(int cardPosition)
        {
            if (DealtCards.Count >= cardPosition + 1) return DealtCards[cardPosition];
            else return null;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Swap the matching card in the hand with a card in the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Swap(Card targetCard, Card cardInDeck)
        {
            for(int i = 0; i < DealtCards.Count; i++)
            {
                if(DealtCards[i] == targetCard)
                {
                    CardBits &= ~(targetCard.Bit);
                    CardBits |= cardInDeck.Bit;
                    DealtCards[i] = cardInDeck;
                    return;
                }
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Add a card to this hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public void AddCard(Card card)
        {
            if((CardBits & card.Bit) > 0)
            {
                throw new ApplicationException("Duplicate Card: " + card);
            }
            CardBits |= card.Bit;
            DealtCards.Add(card);
            _evaluated = false;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Do some preprossessing of the cards before evaluation
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card[] PrepCards()
        {
            var cards = DealtCards.ToArray();

            // Use a custom insertion sort for fastest sorting on this
            // small array.
            for(int i = 1; i < cards.Length; i++)
            {
                for(int j = i-1, k = i; j >= 0 && cards[j].Rank < cards[k].Rank; j--, k--)
                {
                    (cards[j], cards[k]) = (cards[k], cards[j]);
                }
            }

            foreach (var card in cards)
            {
                _suitBits[(int)card.Suit] |= (int)card.Rank;
                _rankBits |= (int)card.Rank;
                // ace is special because it can be the "1" in a strait
                if (card.Rank == Rank.Ace)
                {
                    _suitBits[(int)card.Suit] |= 1;
                    _rankBits |= 1;
                }
                _suitCounts[(int)card.Suit]++;
            }
            return cards;
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Remove cards from the hand, except for the number of cards you specify.  The
        /// cards will be removed last in, first out order
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void ClearAllBut(int keep)
        {
            var pocketCards = DealtCards.Take(keep);
            DealtCards.Clear();

            _suitBits = new int[4];
            _suitCounts = new int[4];
            _rankBits = 0;
            _evaluated = false;
            CardBits = 0;
            foreach(var card in pocketCards)
            {
                AddCard(card);
            }
        }

        List<Rank> _pairs = new List<Rank>(3);
        List<Rank> _kickers = new List<Rank>(7);

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Force the hand to be evaluated if it is not already
        /// </summary>
        //------------------------------------------------------------------------------------
        private void Evaluate()
        {
            if (_evaluated) return;
            _evaluated = true;
            var cards = PrepCards();
            _highCards.Clear();

            int straightMask = 0x3e00;
            Suit flushSuit = Suit.None;

            // Royal flush?
            for (int i = 0; i < 4; i++)
            {
                if (_suitCounts[i] > 4)
                {
                    flushSuit = (Suit)i;
                    if ((_suitBits[i] & straightMask) == straightMask)
                    {
                        _value = HandType.RoyalFlush;
                        return;
                    }
                    break;
                }
            }

            // Strait flush?
            if (flushSuit != Suit.None)
            {
                for (int r = (int)Rank.King; r >= (int)Rank._5; r /= 2)
                {
                    straightMask >>= 1;
                    if ((_suitBits[(int)flushSuit] & straightMask) == straightMask)
                    {
                        _value = HandType.StraightFlush;
                        _highCards.Insert(0, (Rank)r);
                        return;
                    }
                }

            }

            var threeOfAKind = Rank.None;
            var fourOfAKindRank = Rank.None;
            _pairs.Clear();
            _kickers.Clear();

            // Find matches
            for (int i = 0; i < cards.Length; i++)
            {
                var rank = cards[i].Rank;
                int count = 1;
                while (i < cards.Length - 1 && cards[i + 1].Rank == rank)
                {
                    count++;
                    i++;
                }
                switch (count)
                {
                    case 1: _kickers.Add(rank); break;
                    case 2: _pairs.Add(rank); break;
                    case 3:
                        {
                            if (threeOfAKind == Rank.None)
                            {
                                threeOfAKind = rank;
                            }
                            else
                            {
                                _pairs.Insert(0, rank);
                            }
                        }
                        break;
                    case 4: fourOfAKindRank = rank; break;
                }
            }

            // Four of a kind?
            if (fourOfAKindRank != Rank.None)
            {
                _value = HandType.FourOfAKind;
                _highCards.Add(fourOfAKindRank);
                if (threeOfAKind != Rank.None) _highCards.Add(threeOfAKind);
                else if (_pairs.Count > 0 && (_kickers.Count == 0 || _pairs[0] > _kickers[0])) _highCards.Add(_pairs[0]);
                else if (_kickers.Count > 0) _highCards.Add(_kickers[0]);
                return;
            }

            // Full house?
            if (threeOfAKind != Rank.None && _pairs.Count > 0)
            {
                _value = HandType.FullHouse;
                _highCards.Add(threeOfAKind);
                _highCards.Add(_pairs[0]);
                return;
            }

            // Flush?
            if (flushSuit != Suit.None)
            {
                _value = HandType.Flush;

                foreach (var card in cards)
                {
                    if (_highCards.Count >= 5) break;
                    if (card.Suit != flushSuit) continue;
                    _highCards.Add(card.Rank);
                }

                return;
            }

            // Straight?
            straightMask = 0x3e00;
            for (int r = (int)Rank.Ace; r >= (int)Rank._5; r /= 2)
            {
                if ((_rankBits & straightMask) == straightMask)
                {
                    _value = HandType.Straight;
                    _highCards.Insert(0, (Rank)r);
                    return;
                }
                straightMask >>= 1;
            }

            // Set?
            if (threeOfAKind != Rank.None)
            {
                _value = HandType.ThreeOfAKind;
                _highCards.Add(threeOfAKind);
                if (_kickers.Count > 0) _highCards.Add(_kickers[0]);
                if (_kickers.Count > 1) _highCards.Add(_kickers[1]);
                return;
            }

            // Two Pair?
            if( _pairs.Count > 1)
            {
                _value = HandType.TwoPair;
                _highCards.Add(_pairs[0]);
                _highCards.Add(_pairs[1]);
                if(_pairs.Count == 3 && _pairs[2] > _kickers[0])
                {
                    _highCards.Add(_pairs[2]);
                }
                else
                {
                    _highCards.Add(_kickers[0]);
                }
                return;
            }

            // Pair?
            if (_pairs.Count > 0)
            {
                _value = HandType.Pair;
                _highCards.Add(_pairs[0]);

                if(_kickers.Count > 0) _highCards.Add(_kickers[0]);
                if (_kickers.Count > 1) _highCards.Add(_kickers[1]);
                if (_kickers.Count > 2) _highCards.Add(_kickers[2]);
                return;
            }


            _value = HandType.HighCard;
            foreach (var card in cards)
            {
                if (_highCards.Count >= 5) break;
                _highCards.Add(card.Rank);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Compare to another hand.  Returns 1 if we beat the other hand, 0 for tie, -1 for lose
        /// </summary>
        //------------------------------------------------------------------------------------
        public int CompareTo(Hand otherHand)
        {
            if(DealtCards.Count != otherHand.DealtCards.Count)
            {
                throw new ApplicationException("Can't compare hands with different card counts.");
            }
            Evaluate();
            otherHand.Evaluate();
            if (this._value > otherHand._value) return 1;
            if (this._value < otherHand._value) return -1;

            if(_highCards.Count != otherHand._highCards.Count)
            {
                throw new ApplicationException($"Highcard count mismatch ({_highCards.Count},{otherHand._highCards.Count}) : [{this}][{otherHand}]");
            }
            for(int i = 0; i < _highCards.Count; i++)
            {
                // TODO: Sometimes highcard array lengths dont match
                if (_highCards[i] > otherHand._highCards[i]) return 1;
                if (_highCards[i] < otherHand._highCards[i]) return -1;
            }

            return 0;
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// ToString
        /// </summary>
        //------------------------------------------------------------------------------------
        public override string ToString()
        {
            Evaluate();
            return $"{_value} ({string.Join(",", _highCards)}) [{string.Join(",", DealtCards)}]";
        }
    }
}
