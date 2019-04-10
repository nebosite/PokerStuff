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

        public Card PocketCard1 => _hands?[0].DealtCards[0];
        public Card PocketCard2 => _hands?[0].DealtCards[1];
        public Card FlopCard1 { get; set; }
        public Card FlopCard2 { get; set; }
        public Card FlopCard3 { get; set; }
        public Card TurnCard { get; set; }
        public Card RiverCard { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        Deck _deck;
        Hand[] _hands;

        public FlashGameModel()
        {
            _deck = new Deck();
        }

        #region Notification

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


        internal void Reset()
        {
            _deck.Reset();
            _deck.Shuffle();
            _hands = new Hand[PlayerCount];
            for(int i = 0; i < PlayerCount; i++)
            {
                _hands[i] = new Hand();
            }

            for(int i = 0; i < 2; i++)
            {
                foreach(var hand in _hands)
                {
                    hand.AddCard(_deck.Draw());
                }

            }

            NotifyAllPropertiesChanged();
        }
    }
}
