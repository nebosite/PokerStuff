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
    class FlashGameModel : BaseModel
    {
        int _playerCount = 5;
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value;
                CalculateOdds();
                Notify(nameof(PlayerCount));
            }
        }

        const int STRENGTH_BUTTON_COUNT = 7;

        public bool[] StrengthEnabled { get; } = new bool[STRENGTH_BUTTON_COUNT];
        public Brush[] StrengthBackground { get; } = new Brush[STRENGTH_BUTTON_COUNT];

        public string Explanation { get; set; }
        public string ProgressText { get; set; }

        public bool CanDealNext =>  /*_currentTurnScore != null &&*/ RiverCard == null;

        public Card PocketCard1 => _playerHand?.GetDealtCard(0);
        public Card PocketCard2 => _playerHand?.GetDealtCard(1);
        public Card FlopCard1 => _playerHand?.GetDealtCard(2);

        public Card FlopCard2 => _playerHand?.GetDealtCard(3);
        public Card FlopCard3 => _playerHand?.GetDealtCard(4);
        public Card TurnCard => _playerHand?.GetDealtCard(5);
        public Card RiverCard => _playerHand?.GetDealtCard(6);

        Deck _deck;
        Hand _playerHand;
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
            _deck = new Deck();
            Reset();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset this model and draw some cards
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void Reset()
        {
            _potentialPoints = 3;
            _currentTurnScore = null;
            _handScore = 0;
            _deck.Reset();
            _deck.Shuffle();
            _playerHand = new Hand();
            _playerHand.AddCard(_deck.Draw());
            _playerHand.AddCard(_deck.Draw());
            
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
            _tries = 0;
            Notify(nameof(StrengthEnabled));
            Notify(nameof(StrengthBackground));
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deal the next cards
        /// </summary>
        //------------------------------------------------------------------------------------
        public void DealNext()
        {
            ResetButtons();
            _currentTurnScore = null;
            _potentialPoints = 3;
            if (FlopCard1 == null)
            {
                PrintProgress("Dealing flop cards...");
                _playerHand.AddCard(_deck.Draw());
                _playerHand.AddCard(_deck.Draw());
                _playerHand.AddCard(_deck.Draw());
            }
            else if (TurnCard == null)
            {
                PrintProgress("Dealing turn card...");
                _playerHand.AddCard(_deck.Draw());
            }
            else if (RiverCard == null)
            { 
                PrintProgress("Dealing river card...");
                _playerHand.AddCard(_deck.Draw());
            }
            else
            {
                PrintProgress("Whoops, this should not happen.");
            }

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


        OddsResults _currentOdds;
        double[] _strengthPartitions = new double[]
        {
            0,5,15,25,40,65,90,100
        };

        int _tries;

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
            var winPercentage = _currentOdds.WinRatio * 100;
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
        /// Figure the odds for the player's hand
        /// </summary>
        //------------------------------------------------------------------------------------
        private void CalculateOdds()
        {
            if (_playerHand == null) return;
            _currentOdds = OddsCalculator.Calculate(_deck, _playerHand, PlayerCount, TimeSpan.FromMilliseconds(100));          
            Debug.WriteLine($"Number of hands examined: {_currentOdds.Iterations}"); 
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Explain the odds of the current hand
        /// </summary>
        //------------------------------------------------------------------------------------
        public void ShowExplanation()
        {
            var output = new StringBuilder();
            output.AppendLine($"Win percentage: { (_currentOdds.WinRatio * 100.0).ToString(".0")}% ");
            output.AppendLine("\r\nHands performance:");
            var villianInfo = _currentOdds.VillianPerformance.Select(p => Tuple.Create(p.Key, p.Value)).OrderByDescending(t => t.Item2).ToArray();
            var playerInfo = _currentOdds.PlayerPerformance.Select(p => Tuple.Create(p.Key, p.Value)).OrderByDescending(t => t.Item2).ToArray();
            var formatter = new FixedFormatter();
            formatter.ColumnWidths.AddRange(new int[] { -8, 25, -8, 25 });
            output.AppendLine("Your Winning Hands                  Winning Opponent Hands");

            for (int i = 0; i < villianInfo.Length; i++)
            {
                output.AppendLine(formatter.Format(
                    $"{(playerInfo[i].Item2 * 100.0 * _currentOdds.WinRatio).ToString("0.")}%",
                    playerInfo[i].Item1.ToString(),
                    $"{(villianInfo[i].Item2 * 100.0).ToString("0.")}%",
                    villianInfo[i].Item1.ToString()
                ));
            }
            Explanation = output.ToString();
            Notify(nameof(Explanation));
        }

        class FixedFormatter
        {
            public List<int> ColumnWidths = new List<int>();
            public int Padding = 1;

            public string Format(params string[] columnValues)
            {
                var line = new StringBuilder();
                for(int i = 0; i < columnValues.Length; i++)
                {
                    var width = 8;
                    if (i < ColumnWidths.Count) width = ColumnWidths[i];
                    var left = true;
                    if(width < 0)
                    {
                        left = false;
                        width = -width;
                    }

                    var text = columnValues[i];
                    if (left)
                    {
                        line.Append(text);
                        line.Append(new string(' ', width - text.Length));
                    }
                    else
                    {
                        line.Append(new string(' ', width - text.Length));
                        line.Append(text);
                    }
                    line.Append(new string(' ', Padding));

                }

                return line.ToString();
            }
        }
    }
}
