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
    public class Card : IEquatable<Card>
    {
        public Suit Suit;
        public Rank Rank;

        public string SuitText
        {
            get
            {
                switch (Suit)
                {
                    case Suit.Clubs: return "§"; 
                    case Suit.Hearts: return "©"; 
                    case Suit.Diamonds: return "¨"; 
                    case Suit.Spades: return "ª"; 

                }
                return "?";
            }
        }

        public string RankText
        {
            get
            {
                switch (Rank)
                {
                    case Rank.Ace: return "A";
                    case Rank.King: return "K";
                    case Rank.Queen: return "Q";
                    case Rank.Jack: return "J";
                    case Rank._10: return "10";
                    case Rank._9: return "9";
                    case Rank._8: return "8";
                    case Rank._7: return "7";
                    case Rank._6: return "6";
                    case Rank._5: return "5";
                    case Rank._4: return "4";
                    case Rank._3: return "3";
                    case Rank._2: return "2";
                }
                return "?";
            }
        }


        /// <summary>
        /// Unique Id for this card, guaranteed to be a single bit in a 64 bit number
        /// </summary>
        public ulong Bit;

        string _originalData;

        public bool CanDraw { get; set; } = true;

        public Card() : this("Ac")
        {
            
        }

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

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Equatable
        /// </summary>
        //------------------------------------------------------------------------------------
        public bool Equals(Card other)
        {
            return Suit == other.Suit && Rank == other.Rank;
        }
    }
}
