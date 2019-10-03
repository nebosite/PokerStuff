using System;
using System.Collections.Generic;
using System.Text;

namespace PokerParts
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// A property bag to hold the data necessary to run Odds calculations on a particular
    /// street and deck.
    /// </summary>
    //------------------------------------------------------------------------------------
    public class OddsWorkUnit
    {
        public OddsResults Odds { get; set; }
        public Deck Deck = new Deck();
        public Hand PlayerHand = new Hand();
        public string Id => PlayerHand.PocketId;
        public bool IsValid { get; private set; } = false;

        public OddsWorkUnit()
        {
        }

        Random _random = new Random();

        public void PickPlayerCards(params Card[] cards)
        {
            foreach(var card in cards)
            {
                PlayerHand.AddCard(card);
                Deck.Draw(card);
            }
        }

        public bool PickPlayerCards(Rank highRank, Rank lowRank, bool offSuit)
        {
            // TODO:  make sure that the card is available.  Switch suits if not.
            // Pick random suits
            var suit1 = Suit.Clubs;
            var suit2 = offSuit ? Suit.Diamonds : suit1;
            var card1 = new Card(highRank, suit1);
            var card2 = new Card(lowRank, suit2);
            if(!Deck.CanDraw(card1, card2))
            {
                suit1 = Suit.Spades;
                suit2 = offSuit ? Suit.Hearts : suit1;
                card1 = new Card(highRank, suit1);
                card2 = new Card(lowRank, suit2);
                if (!Deck.CanDraw(card1, card2))
                {
                    suit1 = Suit.Diamonds;
                    suit2 = offSuit ? Suit.Spades : suit1;
                    card1 = new Card(highRank, suit1);
                    card2 = new Card(lowRank, suit2);
                }
                if (!Deck.CanDraw(card1, card2))
                {
                    return false;
                }
            }
            PlayerHand.AddCard(card1);
            PlayerHand.AddCard(card2);
            Deck.Draw(PlayerHand.DealtCards[0]);
            Deck.Draw(PlayerHand.DealtCards[1]);
            IsValid = true;
            return true;
        }
    }

}
