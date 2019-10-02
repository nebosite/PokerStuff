using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows.Media;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Handles a flash card style of learning poker odds
    /// </summary>
    //------------------------------------------------------------------------------------
    class FlashGameModel : BaseGameAnalysisModel
    {
        const int STRENGTH_BUTTON_COUNT = 7;

        public bool[] StrengthEnabled { get; } = new bool[STRENGTH_BUTTON_COUNT];
        public Brush[] StrengthBackground { get; } = new Brush[STRENGTH_BUTTON_COUNT];

        public string Explanation { get; set; }
        public string ProgressText { get; set; }

        int _handScore = 0;
        int? _currentTurnScore;
        int _potentialPoints;


        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public FlashGameModel()
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
            _potentialPoints = 3;
            _currentTurnScore = null;
            _handScore = 0;
            
            Explanation = "Click 'Explain' to see stats here.";
            ProgressText = "Starting new hand.  Guess the strength of your current hand. The probability is based on all players continuing to the river.\r\n";
            CalculateOdds();
            ResetButtons();
            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset the strength buttons
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void ResetButtons()
        {
            for(int i = 0; i < STRENGTH_BUTTON_COUNT; i++)
            {
                StrengthEnabled[i] = true;
                StrengthBackground[i] = Brushes.LightGray;
            }
            NotifyPropertyChanged(nameof(StrengthEnabled));
            NotifyPropertyChanged(nameof(StrengthBackground));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deal the next cards
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void DealNext()
        {
            ResetButtons();
            _currentTurnScore = null;
            _potentialPoints = 3;
            base.DealNext();
         
            CalculateOdds();
            NotifyAllPropertiesChanged();
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Re-run odds calculation
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void Recalculate()
        {
            CalculateOdds();
            ShowExplanation();
            NotifyAllPropertiesChanged();
        }

        double[] _strengthPartitions = new double[]
        {
            0,5,15,25,40,65,90,100
        };

        //------------------------------------------------------------------------------------
        /// <summary>
        /// THis is the user picking how strong the current hand is
        /// </summary>
        //------------------------------------------------------------------------------------
        public void SelectStrength(int strength)
        {
            if (!StrengthEnabled[strength] || _currentTurnScore != null) return;
            var low = _strengthPartitions[strength];
            var high = _strengthPartitions[strength + 1];
            var overlapRatio = 0.2;

            if(strength > 0)
            {
                low = _strengthPartitions[strength]
                    - (_strengthPartitions[strength] - _strengthPartitions[strength - 1]) * overlapRatio;
            }
            if (strength < _strengthPartitions.Length - 2)
            {
                high = _strengthPartitions[strength+1]
                    + (_strengthPartitions[strength+2] - _strengthPartitions[strength + 1]) * overlapRatio;
            }

            StrengthEnabled[strength] = false;
            var winPercentage = Odds.WinRatio * 100;
            if(winPercentage < low)
            {
                StrengthBackground[strength] = Brushes.Red;
                PrintProgress("Too High.");
                _potentialPoints -= 2;
                
            }
            else if(winPercentage > high)
            {
                StrengthBackground[strength] = Brushes.Red;
                PrintProgress("Too Low.");
                _potentialPoints -= 2;

            }
            else
            {
                PrintProgress($"Nice!  Probability of a win is {winPercentage.ToString(".0")}%");
                StrengthBackground[strength] = Brushes.Green;
                _currentTurnScore = _potentialPoints;
                _handScore += _potentialPoints;
                PrintProgress($"Current Score: {_handScore}");
            }

            if (_potentialPoints < 0) _potentialPoints = 0;


            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Print Progress information
        /// </summary>
        //------------------------------------------------------------------------------------
        void PrintProgress(string message)
        {
            ProgressText = message + "\r\n" + ProgressText;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Explain the odds of the current hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ShowExplanation()
        {
            Explanation = Odds.GetExplanation(); 
            NotifyPropertyChanged(nameof(Explanation));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Handle Hand change
        /// </summary>
        //------------------------------------------------------------------------------------
        public override void HandleHandChanged()
        {
            Recalculate();
        }
    }
}
