using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParts
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades,
        None
    }

    public enum Rank
    {
        None = 0,
        _2 = 0x0002,
        _3 = 0x0004,
        _4 = 0x0008,
        _5 = 0x0010,
        _6 = 0x0020,
        _7 = 0x0040,
        _8 = 0x0080,
        _9 = 0x0100,
        _10 = 0x0200,
        Jack = 0x0400,
        Queen = 0x0800,
        King = 0x1000,
        Ace = 0x2000,
    }

    public class Card 
    {
        public Suit Suit;
        public Rank Rank;

        string _originalData;

        public Card(string data)
        {
            data = data.Trim().ToUpper();
            _originalData = data;
            if (data.Length != 2) throw new ApplicationException("Bad card specifier: " + data);
            Init(data[0], data[1]);
        }

        public Card(char rank, char suit)
        {
            Init(rank, suit);
            if (_originalData == null) _originalData = "" + rank + suit;
        }

        void Init(char rank, char suit)
        {
            switch (rank)
            {
                case '2': Rank = Rank._2; break;
                case '3': Rank = Rank._3; break;
                case '4': Rank = Rank._4; break;
                case '5': Rank = Rank._5; break;
                case '6': Rank = Rank._6; break;
                case '7': Rank = Rank._7; break;
                case '8': Rank = Rank._8; break;
                case '9': Rank = Rank._9; break;
                case 't':
                case 'T':
                case '0': Rank = Rank._10; break;
                case 'j':
                case 'J': Rank = Rank.Jack; break;
                case 'q':
                case 'Q': Rank = Rank.Queen; break;
                case 'k':
                case 'K': Rank = Rank.King; break;
                case '1':
                case 'a':
                case 'A': Rank = Rank.Ace; break;
                default: throw new ApplicationException("Bad Rank Specifier: " + rank);
            }
            switch (suit)
            {
                case 'c':
                case 'C': Suit = Suit.Clubs; break;
                case 'd':
                case 'D': Suit = Suit.Diamonds; break;
                case 'h':
                case 'H': Suit = Suit.Hearts; break;
                case 's':
                case 'S': Suit = Suit.Spades; break;
                default: throw new ApplicationException("Bad Suit Specifier: " + suit);
            }
        }

        public override string ToString()
        {
            return _originalData;
        }
    }
}
