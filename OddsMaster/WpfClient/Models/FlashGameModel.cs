using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Handles a flash card style of learning poker odds
    /// </summary>
    //------------------------------------------------------------------------------------
    class FlashGameModel : INotifyPropertyChanged
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

        public string Explanation { get; set; }

        public Card PocketCard1 => _playerHand?.DealtCards[0];
        public Card PocketCard2 => _playerHand?.DealtCards[1];
        public Card FlopCard1 { get; set; }

        public Card FlopCard2 { get; set; }
        public Card FlopCard3 { get; set; }
        public Card TurnCard { get; set; }
        public Card RiverCard { get; set; }

        Deck _deck;
        Hand _playerHand;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public FlashGameModel()
        {
            _deck = new Deck();
        }

        #region Notification

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void NotifyAllPropertiesChanged()
        {
            foreach(var property in GetType().GetProperties())
            {
                Notify(property.Name);
            }
        }

        #endregion

        internal void UserSelectedStrength(string strength)
        {
            
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset this model and draw some cards
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void Reset()
        {
            _deck.Reset();
            _deck.Shuffle();

            _playerHand = new Hand();
            _playerHand.AddCard(_deck.Draw());
            _playerHand.AddCard(_deck.Draw());
            FlopCard1 = _deck.Draw();
            FlopCard2 = _deck.Draw();
            FlopCard3 = _deck.Draw();
            _playerHand.AddCard(FlopCard1);
            _playerHand.AddCard(FlopCard2);
            _playerHand.AddCard(FlopCard3);
            Explanation = "Click re-calc to see stats here.";
                

            CalculateOdds(false);

            NotifyAllPropertiesChanged();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Re-run odds calculation
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void Recalculate()
        {
            CalculateOdds(true);

            NotifyAllPropertiesChanged();
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Figure the odds for the player's hand
        /// </summary>
        //------------------------------------------------------------------------------------
        private void CalculateOdds(bool showReasons)
        {
            var odds = OddsCalculator.Calculate(_deck, _playerHand, PlayerCount, TimeSpan.FromMilliseconds(300));
            
            Debug.WriteLine($"Number of hands examined: {odds.Iterations}"); 

            var output = new StringBuilder();
            output.AppendLine($"Win percentage: { (odds.WinRatio * 100.0).ToString(".0")}% ");
            output.AppendLine("\r\nHands performance:");

            var villianInfo = odds.VillianPerformance.Select(p => Tuple.Create(p.Key, p.Value)).OrderByDescending(t => t.Item2).ToArray();
            var playerInfo = odds.PlayerPerformance.Select(p => Tuple.Create(p.Key, p.Value)).OrderByDescending(t => t.Item2).ToArray();
            var formatter = new FixedFormatter();
            formatter.ColumnWidths.AddRange(new int[] { -8, 25, -8, 25 });
            output.AppendLine("Your Winning Hands                  Winning Opponent Hands");

            for(int i = 0; i < villianInfo.Length; i++)
            {
                output.AppendLine(formatter.Format(
                    $"{(playerInfo[i].Item2 * 100.0 * odds.WinRatio).ToString("0.")}%",
                    playerInfo[i].Item1.ToString(),
                    $"{(villianInfo[i].Item2 * 100.0 ).ToString("0.")}%",
                    villianInfo[i].Item1.ToString()
                ));

            }

            if(showReasons)
            {
                Explanation = output.ToString();

            }
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
