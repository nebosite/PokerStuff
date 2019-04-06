﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerOdds;

namespace Tests
{
    [TestClass]
    public class HandTest
    {
        static class Hands
        {
            public const string RoyalFlushDiamonds = "AD,JD,0D,KD,QD";
            public const string RoyalFlushHearts = "Ah,Jh,0h,Kh,Qh";
            public const string StraightFlushDiamondsKing = "9D,JD,0D,KD,QD";
            public const string StraightFlushHearts9 = "9h,8h,7h,6h,5h";
            public const string FourofAKindAce = "Ad,Ah,Ac,As,3h";
            public const string FourofAKind9 = "9d,9h,9c,9s,3h";
            public const string FullHouseAce9 = "Ad,Ah,Ac,9s,9h";
            public const string FullHouseKingJack = "Kd,Kh,Kc,Js,Jh";
            public const string FlushSpadesAceHigh = "3s,As,0s,Qs,9s";
            public const string FlushHearts10High = "0h,9h,2h,7h,6h";
            public const string StraightAceHigh = "As,kh,qh,jc,0c";
            public const string StraightJackHigh = "Js,0h,9c,8c,7c";
            public const string Straight5High = "5h,4d,3d,2c,ac";
            public const string ThreeOfAKindAce = "Ad,Ah,Ac,4s,9h";
            public const string ThreeOfAKind4 = "Ad,4h,4c,4s,9h";
            public const string TwoPairAce8 = "Ad,Ac,8d,8h,4d";
            public const string TwoPairQueen9 = "Qd,Qc,9d,9h,4d";
            public const string PairAce = "Ac,Ad,3h,5h,jc";
            public const string Pair8 = "8c,8d,3h,5h,jc";
            public const string HighCardAce = "Ac,jd,8s,4s,3h";
            public const string HighCardJack = "2c,jd,8s,4s,3h";

            // Hands by rank: =============================================
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

        void AssertHand(HandType expectedType, Rank expectedHighCard, string cards)
        {
            var target = new Hand(cards.Split(','));
            Assert.AreEqual(expectedType, target.Value, target.ToString());
            Assert.AreEqual(expectedHighCard, target.HighCard, target.ToString());
        }

        [TestMethod]
        public void CompareTo_ScoresRoyalFlush_Correctly()
        {
            AssertHand(HandType.RoyalFlush, Rank.None, Hands.RoyalFlushDiamonds);

            // Royal Flush - beats everything, ties self
            TestHands(0, Hands.RoyalFlushDiamonds, Hands.RoyalFlushHearts);
            TestHands(1, Hands.RoyalFlushHearts, Hands.StraightFlushDiamondsKing);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.FourofAKindAce);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.FullHouseAce9);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.FlushSpadesAceHigh);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.StraightAceHigh);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.ThreeOfAKindAce);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.TwoPairAce8);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.PairAce);
            TestHands(1, Hands.RoyalFlushDiamonds, Hands.HighCardAce);

            // No TieBreakers
        }

        [TestMethod]
        public void CompareTo_ScoresStraightFlush_Correctly()
        {
            AssertHand(HandType.StraightFlush, Rank.King, Hands.StraightFlushDiamondsKing);

            TestHands(-1, Hands.StraightFlushDiamondsKing, Hands.RoyalFlushHearts);
            TestHands(0, Hands.StraightFlushDiamondsKing, Hands.StraightFlushDiamondsKing);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.FourofAKindAce);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.FullHouseAce9);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.FlushSpadesAceHigh);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.StraightAceHigh);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.ThreeOfAKindAce);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.TwoPairAce8);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.PairAce);
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.HighCardAce);

            // Tie Breaker is High Card
            TestHands(1, Hands.StraightFlushDiamondsKing, Hands.StraightFlushHearts9);
        }


        [TestMethod]
        public void CompareTo_ScoresFourOfAKind_Correctly()
        {
            AssertHand(HandType.FourOfAKind, Rank._9, Hands.FourofAKind9);

            TestHands(-1, Hands.FourofAKind9, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.FourofAKind9, Hands.StraightFlushDiamondsKing);
            TestHands(0, Hands.FourofAKind9, Hands.FourofAKind9);
            TestHands(1, Hands.FourofAKind9, Hands.FullHouseAce9);
            TestHands(1, Hands.FourofAKind9, Hands.FlushSpadesAceHigh);
            TestHands(1, Hands.FourofAKind9, Hands.StraightAceHigh);
            TestHands(1, Hands.FourofAKind9, Hands.ThreeOfAKindAce);
            TestHands(1, Hands.FourofAKind9, Hands.TwoPairAce8);
            TestHands(1, Hands.FourofAKind9, Hands.PairAce);
            TestHands(1, Hands.FourofAKind9, Hands.HighCardAce);

            // Tie Breaker is High Card
            TestHands(1, Hands.FourofAKindAce, Hands.FourofAKind9);

            var foakAceWith8Kicker = "8d,Ah,Ad,As,Ac";
            var foakAceWith9Kicker = "Ah,Ad,As,Ac,9c";
            TestHands(1, foakAceWith9Kicker, foakAceWith8Kicker);

            // Community card counts for both
            foakAceWith8Kicker += ",Kc";
            foakAceWith9Kicker += ",Kc";
            TestHands(0, foakAceWith9Kicker, foakAceWith8Kicker);
        }

        [TestMethod]
        public void CompareTo_ScoresFullHouse_Correctly()
        {
            AssertHand(HandType.FullHouse, Rank.King, Hands.FullHouseKingJack);

            TestHands(-1, Hands.FullHouseKingJack, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.FullHouseKingJack, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.FullHouseKingJack, Hands.FourofAKind9);
            TestHands(0, Hands.FullHouseKingJack, Hands.FullHouseKingJack);
            TestHands(1, Hands.FullHouseKingJack, Hands.FlushSpadesAceHigh);
            TestHands(1, Hands.FullHouseKingJack, Hands.StraightAceHigh);
            TestHands(1, Hands.FullHouseKingJack, Hands.ThreeOfAKindAce);
            TestHands(1, Hands.FullHouseKingJack, Hands.TwoPairAce8);
            TestHands(1, Hands.FullHouseKingJack, Hands.PairAce);
            TestHands(1, Hands.FullHouseKingJack, Hands.HighCardAce);

            // Tie Breaker is High Card
            TestHands(1, Hands.FullHouseAce9, Hands.FullHouseKingJack);

            // Two 3ok's should resolve to the best hand
            var fh9Jack = "9h,9c,9d,jh,jd";
            TestHands(-1, fh9Jack + ",2h", fh9Jack + ",jc");
        }

        [TestMethod]
        public void CompareTo_ScoresFlush_Correctly()
        {
            AssertHand(HandType.Flush, Rank.Ace, Hands.FlushSpadesAceHigh);

            TestHands(-1, Hands.FlushSpadesAceHigh, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.FlushSpadesAceHigh, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.FlushSpadesAceHigh, Hands.FourofAKind9);
            TestHands(-1, Hands.FlushSpadesAceHigh, Hands.FullHouseAce9);
            TestHands(0, Hands.FlushSpadesAceHigh, Hands.FlushSpadesAceHigh);
            TestHands(1, Hands.FlushSpadesAceHigh, Hands.StraightAceHigh);
            TestHands(1, Hands.FlushSpadesAceHigh, Hands.ThreeOfAKind4);
            TestHands(1, Hands.FlushSpadesAceHigh, Hands.TwoPairAce8);
            TestHands(1, Hands.FlushSpadesAceHigh, Hands.PairAce);
            TestHands(1, Hands.FlushSpadesAceHigh, Hands.HighCardAce);

            // Tie Breaker is High Cards on down
            TestHands(1, Hands.FlushSpadesAceHigh, Hands.FlushHearts10High);

            var winner = "0d,8d,7d,6d,5d";
            var loser =  "0d,8d,7d,6d,4d";
            TestHands(1, winner, loser);

            // Community card counts for both
            winner += ",Kd";
            loser += ",Kd";
            TestHands(0, winner, loser);
        }

        [TestMethod]
        public void CompareTo_ScoresStraight_Correctly()
        {
            AssertHand(HandType.Straight, Rank.Ace, Hands.StraightAceHigh);
            AssertHand(HandType.Straight, Rank._5, Hands.Straight5High);

            // Royal Flush - beats everything, ties self
            TestHands(-1, Hands.StraightAceHigh, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.StraightAceHigh, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.StraightAceHigh, Hands.FourofAKind9);
            TestHands(-1, Hands.StraightAceHigh, Hands.FullHouseAce9);
            TestHands(-1, Hands.StraightAceHigh, Hands.FlushSpadesAceHigh);
            TestHands(0, Hands.StraightAceHigh, Hands.StraightAceHigh);
            TestHands(1, Hands.StraightAceHigh, Hands.ThreeOfAKind4);
            TestHands(1, Hands.StraightAceHigh, Hands.TwoPairAce8);
            TestHands(1, Hands.StraightAceHigh, Hands.PairAce);
            TestHands(1, Hands.StraightAceHigh, Hands.HighCardAce);
        }

        [TestMethod]
        public void CompareTo_ScoresThreeOfAKind_Correctly()
        {
            AssertHand(HandType.ThreeOfAKind, Rank._4, Hands.ThreeOfAKind4);

            // Royal Flush - beats everything, ties self
            TestHands(-1, Hands.ThreeOfAKindAce, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.ThreeOfAKindAce, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.ThreeOfAKindAce, Hands.FourofAKind9);
            TestHands(-1, Hands.ThreeOfAKindAce, Hands.FullHouseAce9);
            TestHands(-1, Hands.ThreeOfAKindAce, Hands.FlushSpadesAceHigh);
            TestHands(-1, Hands.ThreeOfAKindAce, Hands.StraightAceHigh);
            TestHands(0, Hands.ThreeOfAKind4, Hands.ThreeOfAKind4);
            TestHands(1, Hands.ThreeOfAKind4, Hands.TwoPairAce8);
            TestHands(1, Hands.ThreeOfAKind4, Hands.PairAce);
            TestHands(1, Hands.ThreeOfAKind4, Hands.HighCardAce);

            // Tie Breaker is High Card
            TestHands(1, Hands.ThreeOfAKindAce, Hands.ThreeOfAKind4);

            // Kickers don't matter (this is impossible)
            var winner = "0d,0s,0c,6d,5d";
            var loser = "0d,0s,0c,6d,4d";
            TestHands(0, winner, loser);

            // Community card counts for both
            winner += ",Kd";
            loser += ",Kd";
            TestHands(0, winner, loser);
        }

        [TestMethod]
        public void CompareTo_ScoresTwoPair_Correctly()
        {
            AssertHand(HandType.TwoPair, Rank.Queen, Hands.TwoPairQueen9);

            // Royal Flush - beats everything, ties self
            TestHands(-1, Hands.TwoPairAce8, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.TwoPairAce8, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.TwoPairAce8, Hands.FourofAKind9);
            TestHands(-1, Hands.TwoPairAce8, Hands.FullHouseAce9);
            TestHands(-1, Hands.TwoPairAce8, Hands.FlushSpadesAceHigh);
            TestHands(-1, Hands.TwoPairAce8, Hands.StraightAceHigh);
            TestHands(-1, Hands.TwoPairAce8, Hands.ThreeOfAKind4);
            TestHands(0, Hands.TwoPairAce8, Hands.TwoPairAce8);
            TestHands(1, Hands.TwoPairQueen9, Hands.PairAce);
            TestHands(1, Hands.TwoPairQueen9, Hands.HighCardAce);

            // Tie Breakers
            TestHands(1, Hands.TwoPairAce8, Hands.TwoPairQueen9);

            // second pair decides
            var winner = "0d,0s,5c,5d,Kh";
            var loser = "0d,0s,4c,4d,Kh";
            TestHands(1, winner, loser);

            // kicker decides
            winner = "0d,0s,5c,5d,Kh";
            loser = "0d,0s,5c,5d,Qh";
            TestHands(1, winner, loser);

            // Community card counts for both
            winner += ",Ad";
            loser += ",Ad";
            TestHands(0, winner, loser);

        }

        [TestMethod]
        public void CompareTo_ScoresSinglePair_Correctly()
        {
            AssertHand(HandType.Pair, Rank._8, Hands.Pair8);

            // Royal Flush - beats everything, ties self
            TestHands(-1, Hands.PairAce, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.PairAce, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.PairAce, Hands.FourofAKind9);
            TestHands(-1, Hands.PairAce, Hands.FullHouseAce9);
            TestHands(-1, Hands.PairAce, Hands.FlushSpadesAceHigh);
            TestHands(-1, Hands.PairAce, Hands.StraightAceHigh);
            TestHands(-1, Hands.PairAce, Hands.ThreeOfAKind4);
            TestHands(-1, Hands.PairAce, Hands.TwoPairAce8);
            TestHands(0, Hands.PairAce, Hands.PairAce);
            TestHands(1, Hands.Pair8, Hands.HighCardAce);

            // Tie Breakers
            TestHands(1, Hands.PairAce, Hands.Pair8);

            // kickers decide
            var winner = "0d,0s,6c,4d,3h";
            var loser = "0d,0s,6c,4d,2h";
            TestHands(1, winner, loser);

            // Community card counts for both
            winner += ",Ad";
            loser += ",Ad";
            TestHands(0, winner, loser);
        }

        [TestMethod]
        public void CompareTo_ScoresHighCard_Correctly()
        {
            AssertHand(HandType.ThreeOfAKind, Rank._4, "AD,9H,4H,4C,4S");
            AssertHand(HandType.HighScard, Rank.Jack, Hands.HighCardJack);

            // Royal Flush - beats everything, ties self
            TestHands(-1, Hands.HighCardAce, Hands.RoyalFlushHearts);
            TestHands(-1, Hands.HighCardAce, Hands.StraightFlushDiamondsKing);
            TestHands(-1, Hands.HighCardAce, Hands.FourofAKind9);
            TestHands(-1, Hands.HighCardAce, Hands.FullHouseAce9);
            TestHands(-1, Hands.HighCardAce, Hands.FlushSpadesAceHigh);
            TestHands(-1, Hands.HighCardAce, Hands.StraightAceHigh);
            TestHands(-1, Hands.HighCardAce, Hands.ThreeOfAKind4);
            TestHands(-1, Hands.HighCardAce, Hands.TwoPairAce8);
            TestHands(-1, Hands.HighCardAce, Hands.PairAce);
            TestHands(0, Hands.HighCardAce, Hands.HighCardAce);

            // Tie Breakers
            TestHands(1, Hands.HighCardAce, Hands.HighCardJack);

            // kickers decide
            var winner = "0d,9s,6c,4d,3h";
            var loser = "0d,9s,6c,4d,2h";
            TestHands(1, winner, loser);

            // Community card counts for both
            winner += ",Ad";
            loser += ",Ad";
            TestHands(0, winner, loser);
        }

        void TestHands(int expectedValue, string winningCards, string losingCards)
        {
            var winningHand = new Hand(winningCards.Split(','));
            var losingHand = new Hand(losingCards.Split(','));

            Assert.AreEqual(expectedValue, winningHand.CompareTo(losingHand), $"comparing {winningHand} to {losingHand}");
            Assert.AreEqual(-expectedValue, losingHand.CompareTo(winningHand), $"comparing {losingHand} to {winningHand}");
            Assert.AreEqual(0, winningHand.CompareTo(winningHand),$"comparing {winningHand} to {winningHand}");
            Assert.AreEqual(0, losingHand.CompareTo(losingHand),$"comparing {losingHand} to {losingHand}");
        }

        
        [TestMethod]
        public void CompareTo_Handles_FiveCardHands()
        {
            // Add tests here for 7 card hands where the last two cards must be ignored in high card situation
            Assert.Inconclusive("There should be no more than 5 cards considered");

        }

    }
}
