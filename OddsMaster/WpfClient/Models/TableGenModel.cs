using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Handles generating tables of poker odds
    /// </summary>
    //------------------------------------------------------------------------------------
    class TableGenModel : BaseModel
    {
        int _playerCount = 5;
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value;
                Notify(nameof(PlayerCount));
            }
        }

        string _genOutput = "Click 'Generate' to output table values";
        public string GenOutput
        {
            get => _genOutput;
            set
            {
                _genOutput = value;
                Notify(nameof(GenOutput));
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public TableGenModel()
        {
        }

        class CardOddsData
        {
            string _cardType;
            double _oddsTotal;
            int _count = 0;

            public CardOddsData(string typeName)
            {
                _cardType = typeName;
            }

            public void AddOdds(double newOdds)
            {
                if(_count != 0 && Math.Abs(_oddsTotal / _count - newOdds) > 3)
                {
                    throw new Exception($"Odds look bad... Old:{_oddsTotal/_count}  New:{newOdds}");
                }

                _oddsTotal += newOdds;
                _count++;
            }

            public override string ToString()
            {
                return _cardType;
            }
        }

        class OddsWorkUnit
        {
            public OddsResults Odds { get; internal set; }
            public Deck Deck = new Deck();
            public Hand PlayerHand = new Hand();
            public string Id => PlayerHand.PocketId;

            public OddsWorkUnit(Rank highRank, Suit highsuit, Rank lowRank, Suit lowsuit)
            {
                PlayerHand.AddCard(new Card(highRank, highsuit));
                PlayerHand.AddCard(new Card(lowRank, lowsuit));
                Deck.Draw(highRank, highsuit);
                Deck.Draw(lowRank, lowsuit);
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate some data
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Generate()
        {
            var result = new Dictionary<string, OddsWorkUnit>();
            Parallel.ForEach<OddsWorkUnit>(GetAllPairs(), (pair) =>
            //foreach(var pair in GetAllPairs())
            {
                pair.Odds = OddsCalculator.Calculate(pair.Deck, pair.PlayerHand, PlayerCount, TimeSpan.FromMilliseconds(0),3000);
                lock(result)
                {
                    result.Add(pair.Id, pair);
                }
                //Debug.WriteLine($"Result: {pair.Id}: Odds: {(int)(pair.Odds.WinRatio * 100)}%  Iterations: {pair.Odds.Iterations}");
            });
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate all the pairs in the grid
        /// </summary>
        //------------------------------------------------------------------------------------
        IEnumerable<OddsWorkUnit> GetAllPairs()
        {
            foreach(Rank highRank in Enum.GetValues(typeof(Rank)))
            {
                if (highRank == Rank.None) continue;
                foreach (Rank lowRank in Enum.GetValues(typeof(Rank)))
                {
                    if (lowRank == Rank.None || lowRank > highRank) continue;
                    var lowsuit = Suit.Diamonds;
                    var highsuit = Suit.Clubs;
                    yield return new OddsWorkUnit(highRank, highsuit, lowRank, lowsuit);
                    if(lowRank != highRank)
                    {
                        highsuit = lowsuit;
                        yield return new OddsWorkUnit(highRank, highsuit, lowRank, lowsuit);
                    }
                }

            }
        }

    }
}
