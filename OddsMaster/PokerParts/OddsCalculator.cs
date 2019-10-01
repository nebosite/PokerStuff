using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PokerParts
{
    public class OddsResults
    {
        /// <summary>
        /// For all the villian hand types that beat the player, what 
        /// is the ratio of all instances that belong to each type?
        /// E.g.:  What percent of my losses were to a Full House?
        /// This helps to identify the types of hands to watch out for.
        /// </summary>
        public Dictionary<ValueType, double> VillianPerformance { get; set; }

        /// <summary>
        /// For the hands that the player wins, what is the ratio of
        /// wins for each hand type?  i.e.:  What percent of my wins
        /// were from Two Pair?
        /// </summary>
        public Dictionary<ValueType, double> PlayerPerformance { get; set; }

        /// <summary>
        /// What ratio of the iterations did the player win outright?
        /// (Ties do not count as wins)
        /// </summary>
        public double WinRatio { get; set; }

        /// <summary>
        /// How many iterations were claculated within the specified
        /// computer time?
        /// </summary>
        public int Iterations { get; internal set; }

        /// <summary>
        /// Number of BBs won over all the games
        /// </summary>
        public int TotalBigBlindsWon { get; set; }
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    /// A class for calculating odds for a hand to win
    /// </summary>
    //------------------------------------------------------------------------------------
    public class OddsCalculator
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// A class for calculating odds for a hand to win
        /// </summary>
        //------------------------------------------------------------------------------------
        public static OddsResults Calculate(Deck deck, Hand playerHand, int playerCount, TimeSpan computeLimit, int minIterations = 1000, BettingProfile bets = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var rand = new Random();

            var oddsOutput = new OddsResults();
            var deckSpot = deck.DrawSpot;
            var hands = new Hand[playerCount];
            var street = new List<Card>(7);
            var winCount = 0;
            int lossCount = 0;

            var villianWinHands = new Dictionary<HandType, int>();
            var playerWinHands = new Dictionary<HandType, int>();
            foreach (HandType valueType in Enum.GetValues(typeof(HandType)))
            {
                villianWinHands[valueType] = 0;
                playerWinHands[valueType] = 0;
            }

            int iterations = 0;
            deck.Shuffle();

            while(stopwatch.Elapsed < computeLimit || iterations < minIterations)
            {
                iterations++;
                int pot = 0;
                for (int j = 0; j < hands.Length; j++)
                {
                    hands[j] = new Hand();
                }

                // Figure out the community cards first
                street.Clear();
                for (int c = 2; c < playerHand.DealtCards.Count; c++)
                {
                    street.Add(playerHand.DealtCards[c]);
                }
                while (street.Count < 5)
                {
                    street.Add(deck.Draw());
                }

                // Player 0 is the user, deal the two hole cards to them
                hands[0].AddCard(playerHand.DealtCards[0]);
                hands[0].AddCard(playerHand.DealtCards[1]);

                // If the betting profile is specified, deal the 
                // cards accordingly
                int playerNumber = 1;
                int maxContribution = 0;
                if(bets != null)
                {
                    void DealRandomSet(int count, List<Card[]> pairs, int potContribution)
                    {
                        while(count > 0 
                            && playerNumber < hands.Length
                            && pairs.Count > 0)
                        {
                            // Try to deal random cards.  Give up if it takes too
                            // long since we might have an impossible situation
                            int tries = 30;
                            while (--tries > 0)
                            {
                                var pair = pairs[rand.Next(pairs.Count)];
                                if (deck.CanDraw(pair))
                                {
                                    hands[playerNumber].AddCard(deck.Draw(pair[0]));
                                    hands[playerNumber].AddCard(deck.Draw(pair[1]));
                                    break;
                                }
                            }
                            

                            // If we can't draw the cards we want, then just put in some random cards
                            if(tries == 0)
                            {
                                hands[playerNumber].AddCard(deck.Draw());
                                hands[playerNumber].AddCard(deck.Draw());
                            }

                            if(potContribution == 0)
                            {
                                hands[playerNumber].IsFolded = true;
                            }

                            pot += potContribution;
                            if (potContribution > maxContribution) maxContribution = potContribution;
                            playerNumber++;
                            count--;
                        }

                    }

                    DealRandomSet(bets.Foldable, bets.FoldablePairs, 0);
                    DealRandomSet(bets.Weak, bets.WeakPairs, 2);
                    DealRandomSet(bets.Regular, bets.RegularPairs, 4);
                    DealRandomSet(bets.Strong, bets.StrongPairs, 10);
                    pot += maxContribution;
                }

                // Include the player's bet
                oddsOutput.TotalBigBlindsWon -= maxContribution;
                pot += maxContribution;
                
                // Now deal random two cards to everyone else
                for (; playerNumber < hands.Length; playerNumber++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        hands[playerNumber].AddCard(deck.Draw());
                    }
                }

                // Now add the street to all hands
                for (int k = 0; k < street.Count; k++)
                {
                    for (int j = 0; j < hands.Length; j++)
                    {
                        if (hands[j].IsFolded) continue;
                        hands[j].AddCard(street[k]);
                    }
                }


                // Now see how we stack up to the other hands
                var win = true;
                for (int j = 1; j < hands.Length; j++)
                {
                    if (hands[j].IsFolded) continue;
                    var result = hands[0].CompareTo(hands[j]);
                    if (result != 1)
                    {
                        win = false;
                    }

                    if (result == -1)
                    {
                        lossCount++;
                        villianWinHands[hands[j].Value]++;
                    }
                }

                if (win)
                {
                    oddsOutput.TotalBigBlindsWon += pot;
                    playerWinHands[hands[0].Value]++;
                    winCount++;
                }

                deck.Reset(deckSpot);
                deck.Shuffle(playerCount * 2 + 5);
            }

            oddsOutput.Iterations = iterations;
            oddsOutput.VillianPerformance = new Dictionary<ValueType, double>();
            oddsOutput.PlayerPerformance = new Dictionary<ValueType, double>();
            foreach (HandType valueType in Enum.GetValues(typeof(HandType)))
            {
                oddsOutput.VillianPerformance[valueType] = (double)villianWinHands[valueType] / lossCount;
                oddsOutput.PlayerPerformance[valueType] = (double)playerWinHands[valueType] / winCount;
            }
            oddsOutput.WinRatio = (double)winCount / iterations;

            return oddsOutput;
        }
    }
}
