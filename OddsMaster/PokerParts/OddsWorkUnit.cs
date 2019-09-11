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

        public void PickPlayerCards(Rank highRank, Rank lowRank, bool offSuit)
        {
            // TODO:  make sure that the card is available.  Switch suits if not.
            // Pick random suits
            var suit1 = Suit.Clubs;
            var suit2 = offSuit ? Suit.Diamonds : suit1;
            PlayerHand.AddCard(new Card(highRank, suit1));
            PlayerHand.AddCard(new Card(lowRank, suit2));
            Deck.Draw(PlayerHand.DealtCards[0]);
            Deck.Draw(PlayerHand.DealtCards[1]);
        }
    }

}
