using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OddsMaster
{
    public class ProfitTableItem : BaseModel
    {
        public string VisibleText => _pivot ? PivotText : NormalText;
        public string PivotText { get; set; }
        public string NormalText { get; set; }
        public Brush VisibleColor => _pivot ? PivotColor : NormalColor;
        public Brush PivotColor { get; set; }
        public Brush NormalColor { get; set; }

        private bool _pivot;
        private bool _isLabel;

        public ProfitTableItem(string label, bool isLabel = false)
        {
            _isLabel = isLabel;
            NormalText = label;
            NormalColor = Brushes.White;
        }

        public override string ToString()
        {
            return NormalText;
        }
    }

    //------------------------------------------------------------------------------------
    /// <summary>
    /// Handles examining the profitability of a hand
    /// </summary>
    //------------------------------------------------------------------------------------
    class ProfitModel : BaseGameAnalysisModel
    {
        public ObservableCollection<ProfitTableItem[]> ProfitRows { get; set; } = new ObservableCollection<ProfitTableItem[]>();

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public ProfitModel()
        {
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset this model and draw some cards
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void Reset()
        {
            base.Reset();

            ProfitRows = new ObservableCollection<ProfitTableItem[]>();
            for (int i = 0; i < 3; i++)
            {
                var newRow = new ProfitTableItem[10];
                for (int j = 1; j < 10; j++)
                {
                    newRow[j] = new ProfitTableItem(j.ToString(), false);
                }
                ProfitRows.Add(newRow);
            }
            ProfitRows[0][0] = new ProfitTableItem("Weak", true);
            ProfitRows[1][0] = new ProfitTableItem("Normal", true);
            ProfitRows[2][0] = new ProfitTableItem("Strong", true);

            Calculate();
            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deal the next cards
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void DealNext()
        {
            base.DealNext();
            
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Perform the calculations to fill the grid
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Calculate()
        {
            // Run a bunch of hands for each pair of cards we want to test
            Parallel.ForEach<ProfitWorkUnit>(GetGridCalculations(), (unit) =>
            {
                unit.Odds = OddsCalculator.Calculate(
                    unit.Deck, 
                    unit.PlayerHand, 
                    PlayerCount, 
                    TimeSpan.FromMilliseconds(0), 
                    1000, 
                    unit.BettingProfile);

                unit.TableCell.NormalText = ((double)unit.Odds.TotalBigBlindsWon / unit.Odds.Iterations).ToString(".0") + " BB";
                unit.TableCell.NotifyAllPropertiesChanged();
            });

            NotifyAllPropertiesChanged();
        }

        public class ProfitWorkUnit : OddsWorkUnit
        {
            public int Strength { get; private set; }
            public int RemainingPlayers { get; private set; }
            public BettingProfile BettingProfile { get; private set; }
            public ProfitTableItem TableCell { get; private set; }

            public ProfitWorkUnit(int strength, int remaining, BettingProfile profile, ProfitTableItem tableCell)
            {
                Strength = strength;
                RemainingPlayers = remaining;
                BettingProfile = profile;
                TableCell = tableCell;
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Perform the calculations to fill the grid
        /// </summary>
        //------------------------------------------------------------------------------------
        public IEnumerable<ProfitWorkUnit> GetGridCalculations()
        {
            var betThreshold = 1.0 / PlayerCount;
            var weakThreshhold = betThreshold / 1.3;
            var strongThreshhold = betThreshold * 1.3;
            var availablePairs = _deck.GetAllAvailablePairs();

            for(int betStrength=0; betStrength <3; betStrength++)
            {
                for(int remainingOpponentCount = 1; remainingOpponentCount < 10; remainingOpponentCount ++)
                {
                    var cell = ProfitRows[betStrength][remainingOpponentCount];
                    cell.NormalText = "--";
                    if (remainingOpponentCount > (PlayerCount - 1))
                    {
                        cell.NotifyAllPropertiesChanged();
                        continue;
                    }

                    var pickThreshold = betThreshold;
                    var bettingProfile = new BettingProfile();
                    bettingProfile.Foldable = PlayerCount - remainingOpponentCount - 1;
                    switch(betStrength)
                    {
                        case 0:
                            bettingProfile.Weak = remainingOpponentCount;
                            pickThreshold = weakThreshhold;
                            break;
                        case 1:
                            bettingProfile.Regular = remainingOpponentCount;
                            break;
                        case 2:
                            bettingProfile.Strong = remainingOpponentCount;
                            pickThreshold = strongThreshhold;
                            break;
                    }

                    var oddsTable = AppModel.PocketHandOdds[PlayerCount];
                    foreach (var pair in availablePairs)
                    {
                        var id = Deck.GetPairType(pair);
                        if (oddsTable[id] > pickThreshold)
                        {
                            switch (betStrength)
                            {
                                case 0: bettingProfile.WeakPairs.Add(pair); break;
                                case 1: bettingProfile.RegularPairs.Add(pair); break;
                                case 2: bettingProfile.StrongPairs.Add(pair); break;
                            }
                        }
                        else
                        {
                            bettingProfile.FoldablePairs.Add(pair);
                        }
                    }

                    // return some work to do
                    var workUnit = new ProfitWorkUnit(
                        betStrength, 
                        remainingOpponentCount, 
                        bettingProfile,
                        cell
                        );
                    workUnit.PickPlayerCards(PlayerHand.Cards.Select(c => c.Card).ToArray());
                    yield return workUnit;

                }
            }


        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle Hand change
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void HandleHandChanged()
        {
            Calculate();
        }

    }
}
