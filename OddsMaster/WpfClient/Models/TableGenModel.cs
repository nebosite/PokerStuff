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
using System.Windows.Controls;

namespace OddsMaster
{

    public class TableDataItem : BaseModel
    {
        public string VisibleText =>  _pivot ? PivotText : NormalText;
        public string PivotText { get; set; }
        public string NormalText { get; set; }
        public Brush VisibleColor =>  _pivot ? PivotColor : NormalColor;
        public Brush PivotColor { get; set; }
        public Brush NormalColor { get; set; }

        private bool _pivot;
        public double WinRatio { get; set; }
        private string _handType;
        private bool _isLabel;

        public TableDataItem(string label, double winRatio, double threshhold, bool isLabel = false)
        {
            WinRatio = winRatio;
            _handType = label;
            _isLabel = isLabel;
            NormalText = isLabel ? label : $"{_handType}:{(int)(winRatio*100)}%";
            NormalColor = Brushes.White;
            if(!isLabel)
            {
                NormalColor = GetRatioColor(winRatio, threshhold);
            }
        }

        public override string ToString()
        {
            return NormalText;
        }

        public void PivotOn(double selectedRatio)
        {
            if (_isLabel) return;
            _pivot = true;
            var multiplier = WinRatio / selectedRatio;
            var log = Math.Log10(multiplier) * 100;
            if (log < -31.99) log = -31.99;
            if (log > 31.99) log = 31.99;
            log /= 32;
            byte r, g, b;
            r = g = b = 0;
            if(log < 0)
            {
                log = log * log * -1;
                r = (byte)(-log * 256);
                g = (byte)(200 + log * 200);
                b = (byte)(255 + log * 256);
            }
            else
            {
                log *= log;
                r = (byte)(255 - log * 256);
                g = 255;               
            }
            if (multiplier < 1) multiplier = -(1 / multiplier);
            PivotText = $"{_handType}: {multiplier.ToString(".0")}";
            PivotColor = new SolidColorBrush(Color.FromRgb(r,g,b));
            NotifyPropertyChanged(nameof(VisibleColor));
            NotifyPropertyChanged(nameof(VisibleText));
        }

        internal void RemovePivot(double threshhold)
        {
            if (_isLabel) return;
            _pivot = false;
            NormalColor = GetRatioColor(WinRatio, threshhold);
            NotifyPropertyChanged(nameof(VisibleColor));
            NotifyPropertyChanged(nameof(VisibleText));
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
            var level1 = threshhold;
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
                ratio = (ratio - level0) / (level1 - level0);
            }
            else if (ratio < level2)
            {
                rl = rh = gl = gh = 255;
                bh = 220;
                ratio = (ratio - level1) / (level2 - level1);
            }
            else
            {
                gl = gh = 255;
                rh = bh = 220;
                ratio = (ratio - level2) / (1 - level2);
            }

            var r = (byte)((rh - rl) * ratio + rl);
            var g = (byte)((gh - gl) * ratio + gl);
            var b = (byte)((bh - bl) * ratio + bl);
            return new SolidColorBrush(Color.FromArgb(255, r, g, b));
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
                NotifyPropertyChanged(nameof(PlayerCount));
                FixBets();
            }
        }

        int _folds = 0;
        public int Folds
        {
            get => _folds;
            set
            {
                _folds = value;
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
                NotifyPropertyChanged(nameof(ThreshholdPercent));
            }
        }

        string _genOutput = "Click 'Generate' to output table values";
        public string GenOutput
        {
            get => _genOutput;
            set
            {
                _genOutput = value;
                NotifyPropertyChanged(nameof(GenOutput));
            }
        }

        public Card FlopCard1 { get; set; }
        public Card FlopCard2 { get; set; }
        public Card FlopCard3 { get; set; }
        public Card TurnCard { get; set; }
        public Card RiverCard { get; set; }

        public ObservableCollection<TableDataItem[]> TableItems { get; set; } = new ObservableCollection<TableDataItem[]>();

        private DataGridCellInfo _selectedCell;
        public DataGridCellInfo SelectedCell
        {
            get { return _selectedCell; }
            set
            {
                _selectedCell = value;
                if(value.Column != null)
                {
                    var items = _selectedCell.Item as TableDataItem[];
                    var cellItem = items[_selectedCell.Column.DisplayIndex];
                    PivotOnCell(cellItem);
                }
                NotifyPropertyChanged(nameof(SelectedCell));
            }
        }
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
            while(_folds + _weakBets + _regularBets + _strongBets > PlayerCount - 1)
            {
                if (_strongBets > 0) _strongBets--;
                else if (_regularBets > 0) _regularBets--;
                else if (_weakBets > 0) _weakBets--;
                else if (_folds > 0) _folds--;
            }

            NotifyPropertyChanged(nameof(Folds));
            NotifyPropertyChanged(nameof(WeakBets));
            NotifyPropertyChanged(nameof(RegularBets));
            NotifyPropertyChanged(nameof(StrongBets));

            _threshholdPercent = 100 / (PlayerCount-(Folds * 1.0));
            NotifyPropertyChanged(nameof(ThreshholdPercent));

        }

        internal void PivotOnCell(TableDataItem selectedCell)
        {
            foreach(var row in TableItems)
            {
                foreach(var cell in row)
                {
                    if(selectedCell != null)
                    {
                        cell.PivotOn(selectedCell.WinRatio);

                    }
                    else
                    {
                        cell.RemovePivot(_threshholdPercent / 100.0);
                    }
                }
            }
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
        /// 
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Generate()
        {
            var result = new Dictionary<string, OddsWorkUnit>();
            var baseThreshhold = _threshholdPercent / 100.0;
            var betThreshold = 1.0 / PlayerCount;

            //     /// Before calculating odds, fill in the betting profile	
            //- Strong = ratio > base * 1.3
            //- regular = ratio > base
            //- weak = ratio > base / 1.3

            var bettingProfile = new OddsCalculator.Bets();
            bettingProfile.Foldable = Folds;
            bettingProfile.Regular = RegularBets;
            bettingProfile.Weak = WeakBets;
            bettingProfile.Strong = StrongBets;
            var weakThreshhold = betThreshold / 1.3;
            var strongThreshhold = betThreshold * 1.3;

            var oddsTable = AppModel.PocketHandOdds[PlayerCount];
            foreach(var pair in GetAllPairs())
            {
                var id = GetId(pair);
                if (oddsTable[id] > strongThreshhold)
                {
                    bettingProfile.StrongPairs.Add(pair);
                }
                else if (oddsTable[id] > betThreshold)
                {
                    bettingProfile.RegularPairs.Add(pair);
                }
                else if (oddsTable[id] > weakThreshhold)
                {
                    bettingProfile.WeakPairs.Add(pair);
                }
                else
                {
                    bettingProfile.FoldablePairs.Add(pair);
                }
            }



            // Run a bunch of hands for each pair of cards we want to test
            Parallel.ForEach<OddsWorkUnit>(GetAllPairWorkUnits(), (pair) =>
            {
                pair.Odds = OddsCalculator.Calculate(pair.Deck, pair.PlayerHand, PlayerCount, TimeSpan.FromMilliseconds(0), 10000, bettingProfile);
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
                TableItems[y][0] = new TableDataItem(highRank.ToString(), 0, 0, isLabel: true);


                for (int x = 0; x < 13; x++)
                {
                    if (x < y) continue;
                    var lowRank = ranks[x];
                    if (x == y)
                    {
                        var key = "" + highRank + lowRank;

                        TableItems[y][x + 1] = new TableDataItem(key, result[key].Odds.WinRatio, baseThreshhold);

                    }
                    else
                    {
                        var key = "" + highRank + lowRank + "o";

                        TableItems[x][y+1] = new TableDataItem(key, result[key].Odds.WinRatio, baseThreshhold);

                        key = "" + highRank + lowRank + "s";

                        TableItems[y][x+1] = new TableDataItem(key, result[key].Odds.WinRatio, baseThreshhold);
                    }
                }

                var tableData = new StringBuilder();
                foreach(var item in result.Values.OrderByDescending(v => v.Odds.WinRatio))
                {
                    tableData.AppendLine($"\"{item.Id}\": \"{item.Odds.WinRatio.ToString(".000")}\",");
                }
                GenOutput = tableData.ToString();
                NotifyPropertyChanged(nameof(TableItems));
            }
    
        }

        private string GetId(Card[] pair)
        {
            // high card is first
            if(pair[0].Rank < pair[1].Rank)
            {
                var temp = pair[0];
                pair[0] = pair[1];
                pair[1] = temp;
            }

            var letter1 = Hand.GetRankChar(pair[0].Rank);
            var letter2 = Hand.GetRankChar(pair[1].Rank);
            var suitLetter = "";
            if (pair[0].Rank != pair[1].Rank)
            {
                suitLetter = pair[0].Suit == pair[1].Suit ? "s" : "o";
            }


            return $"{letter1}{letter2}{suitLetter}";
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate all possible pairs of cards from the deck
        /// </summary>
        //------------------------------------------------------------------------------------
        IEnumerable<Card[]> GetAllPairs()
        {
            var deck = new Deck();
            int count = 0;
            for(int i = 0; i < deck.AllCards.Length; i++)
            {
                for (int j = i+1; j < deck.AllCards.Length; j++)
                {
                    count++;
                    yield return new Card[] { deck.AllCards[i], deck.AllCards[j] };
                }
            }

        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate all the pairs in the grid
        /// </summary>
        //------------------------------------------------------------------------------------
        IEnumerable<OddsWorkUnit> GetAllPairWorkUnits()
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

        DataGridCellInfo _lastSelection;
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate all the pairs in the grid
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void MaybeDeselect()
        {
            if(SelectedCell == _lastSelection)
            {
                PivotOnCell(null); 
            }
            _lastSelection = SelectedCell;
        }
    }
}
