using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerOdds
{
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

    public class Hand
    {
        List<Card> _cards = new List<Card>(7);

        int[] _suitBits = new int[4];
        int[] _suitCounts = new int[4];
        int _maxSuitCount;
        int _rankBits;

        public Hand(string[] cards = null)
        {
            if (cards == null) return;
            foreach (var card in cards) AddCard(new Card(card));
        }

        public void AddCard(Card card)
        {
            bool cardAdded = false;
            for (int i = 0; i < _cards.Count; i++)
            {
                var rank = _cards[i].Rank;
                if (card.Rank > rank)
                {
                    _cards.Insert(i, card);
                    cardAdded = true;
                    break;
                }
                if (card.Rank == rank && card.Suit == _cards[i].Suit)
                {
                    throw new ApplicationException("Duplicate Card: " + card);
                }
            }
            if (!cardAdded) _cards.Add(card);

            _suitBits[(int)card.Suit] |= (int)card.Rank;
            _rankBits |= (int)card.Rank;
            // ace is special because it can be the "1" in a strait
            if (card.Rank == Rank.Ace)
            {
                _suitBits[(int)card.Suit] |= 1;
                _rankBits |= 1;
            }
            _suitCounts[(int)card.Suit]++;
            if (_suitCounts[(int)card.Suit] > _maxSuitCount) _maxSuitCount++;
            _evaluated = false;
            // Figure out the value of the hand here
            // Fill bit arrays by rank (A is both high and low)
            //  OR bit arrays to look for mixed suits (bitshift the pattern down)
            //  use AND on bit patterns to find straights
            //  Count suits to find flushes
            // Find Counts of 4 of a kind, 3ok, and pairs, mark high cards


        }

        HandType _value;
        public HandType Value
        {
            get
            {
                Evaluate();
                return _value;
            }
        }

        List<Rank> _highCards = new List<Rank>(5);
        public Rank HighCard
        {
            get
            {
                Evaluate();
                return _highCards.Count == 0 ? Rank.None : _highCards[0];
            }
        }


        bool _evaluated = false;
        public void Evaluate()
        {
            if (_evaluated) return;
            _evaluated = true;
            _highCards.Clear();

            int straightMask = 0x3e00;
            Suit flushSuit = Suit.None;
            if (_maxSuitCount > 4)
            {
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
                    }
                }

                // Strait flush?
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

            var pairs = new List<Rank>(3);
            var threeOfAKind = Rank.None;
            var fourOfAKindRank = Rank.None;
            var kickers = new List<Rank>(7);

            // Find matches
            for (int i = 0; i < _cards.Count; i++)
            {
                var rank = _cards[i].Rank;
                int count = 1;
                while (i < _cards.Count - 1 && _cards[i + 1].Rank == rank)
                {
                    count++;
                    i++;
                }
                switch (count)
                {
                    case 1: kickers.Add(rank); break;
                    case 2: pairs.Add(rank); break;
                    case 3:
                        {
                            if (threeOfAKind == Rank.None)
                            {
                                threeOfAKind = rank;
                            }
                            else
                            {
                                pairs.Insert(0, rank);
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
                return;
            }

            // Full house?
            if (threeOfAKind != Rank.None && pairs.Count > 0)
            {
                _value = HandType.FullHouse;
                _highCards.Add(threeOfAKind);
                _highCards.Add(pairs[0]);
                return;
            }

            // Flush?
            if (flushSuit != Suit.None)
            {
                _value = HandType.Flush;

                foreach (var card in _cards)
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
                return;
            }

            // Two Pair?
            if( pairs.Count > 1)
            {
                _value = HandType.TwoPair;
                _highCards.Add(pairs[0]);
                _highCards.Add(pairs[1]);
                if(pairs.Count == 3 && pairs[2] > kickers[0])
                {
                    _highCards.Add(pairs[2]);
                }
                else
                {
                    _highCards.Add(kickers[0]);
                }
                return;
            }

            // Pair?
            if (pairs.Count > 0)
            {
                _value = HandType.Pair;
                _highCards.Add(pairs[0]);

                _highCards.Add(kickers[0]);
                _highCards.Add(kickers[1]);
                _highCards.Add(kickers[2]);
                return;
            }


            _value = HandType.HighCard;
            foreach (var card in _cards)
            {
                if (_highCards.Count >= 5) break;
                _highCards.Add(card.Rank);
            }
        }

        public int CompareTo(Hand otherHand)
        {
            if(_cards.Count != otherHand._cards.Count)
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

            // Royal Flush
            // Straight Flush
            // Four of a kind
            // Full house
            // Flush
            // Straight
            // 3oak
            // 2pair
            // pair
            // high card

        }


        public override string ToString()
        {
            Evaluate();
            return $"{_value} ({string.Join(",", _highCards)}) [{string.Join(",", _cards)}]";
        }
    }
}
