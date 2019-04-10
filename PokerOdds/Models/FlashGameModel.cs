using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerOdds
{
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

        internal void Reset()
        {
            _deck.Reset();
            _deck.Shuffle();

            _playerHand = new Hand();
            _playerHand.AddCard(_deck.Draw());
            _playerHand.AddCard(_deck.Draw());

            CalculateOdds(2);

            NotifyAllPropertiesChanged();
        }

        private void CalculateOdds(int keep)
        {
            var deckSpot = _deck.Spot;
            var stopwatch = Stopwatch.StartNew();
            var hands = new Hand[PlayerCount - 1];
            for (int i = 0; i < 100000; i++)
            {
                for (int j = 0; j < hands.Length; j++)
                {
                    hands[j] = new Hand();
                }

                _deck.Reset(deckSpot);
                _deck.Shuffle();

                for(int k = 0; k < 2; k++)
                {
                    for (int j = 0; j < hands.Length; j++)
                    {
                        hands[j].AddCard(_deck.Draw());
                    }
                }

                int streetCount = _playerHand.DealtCards.Count - 2;
                int remaining = 5 - streetCount;

                for (int j = 0; j < hands.Length; j++)
                {
                    //hands[j].AddCard(flop[0]);
                    //hands[j].AddCard(flop[1]);
                    //hands[j].AddCard(flop[2]);
                    //hands[j].AddCard(turn);
                    //hands[j].AddCard(river);
                }

                for (int j = 1; j < hands.Length; j++)
                {
                    hands[0].CompareTo(hands[j]);
                }
            }

            var elapsed = stopwatch.Elapsed;
            Debug.WriteLine("Elapsed Milliseconds: " + (elapsed.TotalSeconds * 1000).ToString(".00"));
        }
    }
}
