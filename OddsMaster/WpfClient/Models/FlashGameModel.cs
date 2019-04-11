using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

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

            NotifyAllPropertiesChanged();
        }


        //------------------------------------------------------------------------------------
        /// <summary>
        /// Figure the odds for the player's hand
        /// </summary>
        //------------------------------------------------------------------------------------
        private void CalculateOdds()
        {
            var deckSpot = _deck.DrawSpot;
            var stopwatch = Stopwatch.StartNew();
            var hands = new Hand[PlayerCount];
            var street = new List<Card>(7);
            var wins = 0;
            int lossCount = 0;

            var villianHands = new Dictionary<HandType, int>();
            foreach (HandType valueType in Enum.GetValues(typeof(HandType)))
            {
                villianHands[valueType] = 0;
            }

            int iterations = 10000;
            _deck.Shuffle();

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < hands.Length; j++)
                {
                    hands[j] = new Hand();
                }

                // Figure out the community cards first
                street.Clear();
                for (int c = 2; c < _playerHand.DealtCards.Count; c++)
                {
                    street.Add(_playerHand.DealtCards[c]);
                }
                while(street.Count < 5)
                {
                    street.Add(_deck.Draw());
                }

                // Deal player cards to dummy player
                hands[0].AddCard(_playerHand.DealtCards[0]);
                hands[0].AddCard(_playerHand.DealtCards[1]);

                // Now deal two cards to everyone else
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 1; j < hands.Length; j++)
                    {
                        hands[j].AddCard(_deck.Draw());
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
                    if (result != 1) win = false;
                    if(result == -1)
                    {
                        lossCount++;
                        villianHands[hands[j].Value]++;
                    }
                }
                if (win) wins++;

                _deck.Reset(deckSpot);
                _deck.Shuffle();
            }

            var elapsed = stopwatch.Elapsed;
            Debug.WriteLine("Elapsed Milliseconds: " + (elapsed.TotalSeconds * 1000).ToString(".00"));

            var output = new StringBuilder();
            output.AppendLine($"Win percentage: { ((wins * 100.0) / iterations).ToString(".0")}% ");
            output.AppendLine("Hands that beat you:");
            foreach(HandType valueType in Enum.GetValues(typeof( HandType)))
            {
                output.AppendLine($"{valueType}: {(villianHands[valueType] * 100.0 / lossCount).ToString(".0")}%");
            }

            Explanation = output.ToString();

        }
    }
}
