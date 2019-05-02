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
        public static OddsResults Calculate(Deck deck, Hand playerHand, int playerCount, TimeSpan computeLimit, int minIterations = 1000)
        {
            var stopwatch = Stopwatch.StartNew();

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

                // Deal player cards to dummy player
                hands[0].AddCard(playerHand.DealtCards[0]);
                hands[0].AddCard(playerHand.DealtCards[1]);

                // Now deal two cards to everyone else
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 1; j < hands.Length; j++)
                    {
                        hands[j].AddCard(deck.Draw());
                    }
                }

                // Now add the street to all hands
                for (int k = 0; k < street.Count; k++)
                {
                    for (int j = 0; j < hands.Length; j++)
                    {
                        hands[j].AddCard(street[k]);
                    }
                }


                // Now see how we stack up to the other hands
                var win = true;
                for (int j = 1; j < hands.Length; j++)
                {
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
                    playerWinHands[hands[0].Value]++;
                    winCount++;
                }

                deck.Reset(deckSpot);
                deck.Shuffle();
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
