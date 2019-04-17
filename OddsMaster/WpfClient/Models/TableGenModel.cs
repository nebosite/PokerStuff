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
using System.Collections.ObjectModel;

namespace OddsMaster
{

    public class TableDataItem
    {
        public string Text { get; set; }
        public Brush CellColor { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

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

        public Card FlopCard1 { get; set; }
        public Card FlopCard2 { get; set; }
        public Card FlopCard3 { get; set; }
        public Card TurnCard { get; set; }
        public Card RiverCard { get; set; }

        public ObservableCollection<TableDataItem[]> TableItems { get; set; } = new ObservableCollection<TableDataItem[]>();

        const string RankString = "AKQJT98765432";

        Deck _deck = new Deck();

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public TableGenModel()
        {
        }

        internal void DealFlop()
        {
            _deck.Reset();
            _deck.Shuffle();
            FlopCard1 = _deck.Draw();
            FlopCard2 = _deck.Draw();
            FlopCard3 = _deck.Draw();

            NotifyAllPropertiesChanged();
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

            bool _offSuit;

            public OddsWorkUnit()
            {
            }

            Random _random = new Random();
            public void PickPlayerCards(Rank highRank, Rank lowRank, bool _offSuit)
            {
                // TODO:  make sure that the card is available.  Switch suits if not.
                // Pick random suits
                var suit1 = Suit.Clubs;
                var suit2 = _offSuit ? Suit.Diamonds : suit1;
                PlayerHand.AddCard(new Card(highRank, suit1));
                PlayerHand.AddCard(new Card(lowRank, suit2));
                Deck.Draw(PlayerHand.DealtCards[0]);
                Deck.Draw(PlayerHand.DealtCards[1]);
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
                TableItems.Add(new TableDataItem[14]);
            }

            for (int y = 0; y < 13; y++)
            {
                var highRank = ranks[y];
                TableItems[y][0] = new TableDataItem()
                {
                    Text = highRank.ToString(),
                    CellColor = Brushes.White
                };

                for (int x = 0; x < 13; x++)
                {
                    if (x < y) continue;
                    var lowRank = ranks[x];
                    if (x == y)
                    {
                        var key = "" + highRank + lowRank;

                        TableItems[y][x+1] =  new TableDataItem()
                        {
                            Text = key + " " + (result[key].Odds.WinRatio * 100).ToString("0.") + "%",
                            CellColor = GetRatioColor(result[key].Odds.WinRatio)
                        };
                    }
                    else
                    {
                        var key = "" + highRank + lowRank + "o";

                        TableItems[x][y+1] = new TableDataItem()
                        {
                            Text = key+ " " + (result[key].Odds.WinRatio * 100).ToString("0.") + "%",
                            CellColor = GetRatioColor(result[key].Odds.WinRatio)
                        };

                        key = "" + highRank + lowRank + "s";

                        TableItems[y][x+1] = new TableDataItem()
                        {
                            Text = key+ " " + (result[key].Odds.WinRatio * 100).ToString("0.") + "%",
                            CellColor = GetRatioColor(result[key].Odds.WinRatio)
                        };
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
                ratio = (ratio - 0.5) * 2;
                var colorRatio = Math.Abs(ratio);
                if (ratio > 0)
                {
                    r = 255;
                    g = b = (byte)(255 - (colorRatio * 255));
                }
                else
                {
                    r = g = (byte)(255 - (colorRatio * 127));
                    b = (byte)(255 - (colorRatio * 80));
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
            OddsWorkUnit GetUnit(Rank highRank, Rank lowRank, bool offSuit)
            {
                var unit = new OddsWorkUnit();
                if (FlopCard1 != null)
                {
                    unit.Deck.Draw(FlopCard1);
                    unit.Deck.Draw(FlopCard2);
                    unit.Deck.Draw(FlopCard3);
                    unit.PlayerHand.AddCard(FlopCard1);
                    unit.PlayerHand.AddCard(FlopCard2);
                    unit.PlayerHand.AddCard(FlopCard3);
                }
                unit.PickPlayerCards(highRank, lowRank, offSuit);
                return unit;
            }

            foreach(Rank highRank in Enum.GetValues(typeof(Rank)))
            {
                if (highRank == Rank.None) continue;
                foreach (Rank lowRank in Enum.GetValues(typeof(Rank)))
                {
                    if (lowRank == Rank.None || lowRank > highRank) continue;
                    yield return GetUnit(highRank, lowRank, offSuit: true);
                    if(lowRank != highRank)
                    {
                        yield return GetUnit(highRank, lowRank, offSuit: false);
                    }
                }

            }
        }

    }
}
