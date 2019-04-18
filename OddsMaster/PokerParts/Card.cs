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
    /// Card Suits
    /// </summary>
    //------------------------------------------------------------------------------------
    public enum Suit
    {
        Clubs = 0,
        Diamonds = 1,
        Hearts = 2,
        Spades = 3,
        None
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    /// Card Ranks
    /// </summary>
    //------------------------------------------------------------------------------------
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

    //------------------------------------------------------------------------------------
    /// <summary>
    /// Card 
    /// </summary>
    //------------------------------------------------------------------------------------
    public class Card 
    {
        public Suit Suit;
        public Rank Rank;
        public ulong Bit;

        string _originalData;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor - Call with two-character card designation.  e.g.:  
        ///     "JD" = Jack of Diamonds
        ///     "0s" = 10 of Spaces 
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card(string twoCharacterCard)
        {
            twoCharacterCard = twoCharacterCard.Trim().ToUpper();
            _originalData = twoCharacterCard;
            if (twoCharacterCard.Length != 2) throw new ApplicationException("Bad card specifier: " + twoCharacterCard);
            Init(twoCharacterCard[0], twoCharacterCard[1]);
            SetBit();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card(char rank, char suit)
        {
            Init(rank, suit);
            if (_originalData == null) _originalData = "" + rank + suit;
            SetBit();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
            if (_originalData == null) _originalData = "" + rank + suit;
            SetBit();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Set a unique bit that corresponds to this card
        /// </summary>
        //------------------------------------------------------------------------------------
        void SetBit()
        {
            Bit = (ulong)(Rank)  << ((int)Suit*16);
        }

        public override int GetHashCode()
        {
            return (int)Bit;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Initialize this card from character designations
        /// </summary>
        //------------------------------------------------------------------------------------
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

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ToString
        /// </summary>
        //------------------------------------------------------------------------------------
        public override string ToString()
        {
            return _originalData;
        }
    }
}
