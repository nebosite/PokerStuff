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
                _threshholdPercent = 100 / PlayerCount;
                Notify(nameof(ThreshholdPercent));
                FixBets();
            }
        }

        int _weakBets = 0;
        public int WeakBets
        {
            get => _weakBets;
            set
            {
                _weakBets = value;
                FixBets();
            }
        }

        int _regularBets = 0;
        public int RegularBets
        {
            get => _regularBets;
            set
            {
                _regularBets = value;
                FixBets();
            }
        }

        int _strongBets = 0;
        public int StrongBets
        {
            get => _strongBets;
            set
            {
                _strongBets = value;
                FixBets();
            }
        }

        double _threshholdPercent = 33;
        public string ThreshholdPercent
        {
            get => _threshholdPercent.ToString();
            set
            {
                if(!double.TryParse(value, out var doubleValue))
                {
                    doubleValue = 33;
                }
                if (doubleValue < 0) doubleValue = 0;
                if (doubleValue > 99) doubleValue = 99;
                _threshholdPercent = doubleValue;
                Notify(nameof(ThreshholdPercent));
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
            PlayerCount = 5;
        }

        void FixBets()
        {
            while(_weakBets + _regularBets + _strongBets > PlayerCount)
            {
                if (_strongBets > 0) _strongBets--;
                else if (_regularBets > 0) _regularBets--;
                else if (_weakBets > 0) _weakBets--;
            }

            Notify(nameof(WeakBets));
            Notify(nameof(RegularBets));
            Notify(nameof(StrongBets));
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
            var baseThreshhold = _threshholdPercent / 100.0;

            // Run a bunch of hands for each pair of cards we want to test
            Parallel.ForEach<OddsWorkUnit>(GetAllPairs(), (pair) =>
            {
                pair.Odds = OddsCalculator.Calculate(pair.Deck, pair.PlayerHand, PlayerCount, TimeSpan.FromMilliseconds(0), 10000);
                lock (result)
                {
                    result.Add(pair.Id, pair);
                }
            });

            // Process the results into a spreadsheet
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
                            CellColor = GetRatioColor(result[key].Odds.WinRatio, baseThreshhold)
                        };
                    }
                    else
                    {
                        var key = "" + highRank + lowRank + "o";

                        TableItems[x][y+1] = new TableDataItem()
                        {
                            Text = key+ " " + (result[key].Odds.WinRatio * 100).ToString("0.") + "%",
                            CellColor = GetRatioColor(result[key].Odds.WinRatio, baseThreshhold)
                        };

                        key = "" + highRank + lowRank + "s";

                        TableItems[y][x+1] = new TableDataItem()
                        {
                            Text = key+ " " + (result[key].Odds.WinRatio * 100).ToString("0.") + "%",
                            CellColor = GetRatioColor(result[key].Odds.WinRatio, baseThreshhold)
                        };
                    }
                }

                var tableData = new StringBuilder();
                foreach(var item in result.Values.OrderByDescending(v => v.Odds.WinRatio))
                {
                    tableData.AppendLine($"\"{item.Id}\": \"{item.Odds.WinRatio.ToString(".000")}\",");
                }
                GenOutput = tableData.ToString();
                Notify(nameof(TableItems));
            }
    
            //------------------------------------------------------------------------------------
            /// <summary>
            /// Create a heatmap color based on the ratio
            /// </summary>
            //------------------------------------------------------------------------------------
            Brush GetRatioColor(double ratio, double threshhold)
            {
                int rl, gl, bl;
                int rh, gh, bh;
                var level0 = threshhold / 1.3;
                var level1 = threshhold ;
                var level2 = threshhold * 1.3;

                rh = gh = bh = 255;
                rl = gl = bl = 0;

                if (ratio < level0)
                {
                    rl = gl = bl = 100;
                    rh = gh = bh = 220;
                    ratio = ratio / level0;
                }
                else if (ratio < level1)
                {
                    rl = rh = 255;
                    gh = bh = 220;
                    ratio = (ratio - level0) / (level1-level0);
                }
                else if (ratio < level2)
                {
                    rl = rh = gl = gh = 255;
                    bh = 220;
                    ratio = (ratio - level1) / (level2-level1);
                }
                else 
                {
                    gl = gh = 255;
                    rh = bh = 220;
                    ratio = (ratio - level2) / (1-level2);
                }

                var r = (byte)((rh - rl) * ratio + rl);
                var g = (byte)((gh - gl) * ratio + gl);
                var b = (byte)((bh - bl) * ratio + bl);
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
