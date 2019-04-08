using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerOdds
{

    class Deck
    {
        Random _random = new Random();
        List<Card> _cards = new List<Card>();
        List<Card> _drawnCards = new List<Card>();
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
        }

        public void Shuffle()
        {
            for (int i = 0; i < _cards.Count - 1; i++)
            {
                var remainingCount = _cards.Count - i - 1;
                var swapIndex = _random.Next(remainingCount) + i + 1;
                var temp = _cards[swapIndex];
                _cards[swapIndex] = _cards[i];
                _cards[i] = temp;
            }
        }

        public Card Draw()
        {
            if (_cards.Count == 0) throw new ApplicationException("Tried to draw from an empty deck.");
            var returnMe = _cards.Last();
            _cards.RemoveAt(_cards.Count - 1);
            _drawnCards.Add(returnMe);
            return returnMe;
        }

        public void Reset()
        {
            _cards.AddRange(_drawnCards);
            _drawnCards.Clear();
        }
    }
}
