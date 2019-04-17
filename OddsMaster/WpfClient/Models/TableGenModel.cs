using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Media;
using System.Data;

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

        //public List<TableDataItem[]> TableItems { get; set; } = new List<TableDataItem[]>();
        public DataTable TableItems { get; set; } = new DataTable();

        const string RankString = "AKQJT98765432";

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public TableGenModel()
        {
            TableItems.Columns.Add(new DataColumn("..", typeof(TableDataItem)));
            for(int i = 0; i < 13; i++)
            {
                TableItems.Columns.Add(new DataColumn(RankString[i].ToString(), typeof(TableDataItem)));
            }
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
                if (_count != 0 && Math.Abs(_oddsTotal / _count - newOdds) > 3)
                {
                    throw new Exception($"Odds look bad... Old:{_oddsTotal / _count}  New:{newOdds}");
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

        public class TableDataItem
        {
            public string Display { get; set; }
            public Brush CellColor { get; set; }
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
            {
                pair.Odds = OddsCalculator.Calculate(pair.Deck, pair.PlayerHand, PlayerCount, TimeSpan.FromMilliseconds(0), 3000);
                lock (result)
                {
                    result.Add(pair.Id, pair);
                }
            });

            var ranks = "AKQJT98765432";
            TableItems.Clear();
            for (int i = 0; i < 13; i++)
            {
                TableItems.Rows.Add(TableItems.NewRow());
            }

            for (int y = 0; y < 13; y++)
            {
                var highRank = ranks[y];
                for (int x = 0; x < 13; x++)
                {
                    if (x < y) continue;
                    var lowRank = ranks[x];
                    if (x == y)
                    {
                        var key = "" + highRank + lowRank;

                        //TableItems[y][x] = new TableDataItem()
                        //{
                        //    Display = key + " " + result[key].Odds.WinRatio.ToString("0.") + "%",
                        //    CellColor = GetRatioColor(result[key].Odds.WinRatio)
                        //};
                    }
                    else
                    {
                        var key = "" + highRank + lowRank + "o";

                        //TableItems[x][y] = new TableDataItem()
                        //{
                        //    Display = key + " " + result[key].Odds.WinRatio.ToString("0.") + "%",
                        //    CellColor = GetRatioColor(result[key].Odds.WinRatio)
                        //};

                        key = "" + highRank + lowRank + "s";

                        //TableItems[y][x] = new TableDataItem()
                        //{
                        //    Display = key + " " + result[key].Odds.WinRatio.ToString("0.") + "%",
                        //    CellColor = GetRatioColor(result[key].Odds.WinRatio)
                        //};
                    }
                }

                Notify(nameof(TableItems));
            }
    
            //------------------------------------------------------------------------------------
            /// <summary>
            /// Create a heatmap color based on the ratio
            /// </summary>
            //------------------------------------------------------------------------------------
            Brush GetRatioColor(double ratio)
            {
                byte r, g, b;
                if (ratio > .5)
                {
                    r = 255;
                    g = b = (byte)(255 - ((ratio - 0.5) * 2 * 255));
                }
                else
                {
                    r = g = (byte)(255 - ((.5 - ratio) * 2 * 255));
                    b = (byte)(255 - ((.5 - ratio) * 2 * 127));
                }
                return new SolidColorBrush(Color.FromArgb(255, r, g, b));
            }
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
