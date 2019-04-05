using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerOdds
{
    class AppModel : INotifyPropertyChanged
    {
        string _playerCard1;
        public string PlayerCard1
        {
            get => _playerCard1;
            set { _playerCard1 = NewCard(value, nameof(PlayerCard1)); }
        }

        string _playerCard2;
        public string PlayerCard2
        {
            get => _playerCard2;
            set { _playerCard2 = NewCard(value, nameof(PlayerCard2)); }
        }

        string _dealerCard1;
        public string DealerCard1
        {
            get => _dealerCard1;
            set { _dealerCard1 = NewCard(value, nameof(DealerCard1)); }
        }

        string _dealerCard2;
        public string DealerCard2
        {
            get => _dealerCard2;
            set { _dealerCard2 = NewCard(value, nameof(DealerCard2)); }
        }

        string _dealerCard3;
        public string DealerCard3
        {
            get => _dealerCard3;
            set { _dealerCard3 = NewCard(value, nameof(DealerCard3)); }
        }

        string _dealerCard4;
        public string DealerCard4
        {
            get => _dealerCard4;
            set { _dealerCard4 = NewCard(value, nameof(DealerCard4)); }
        }

        string _dealerCard5;
        public string DealerCard5
        {
            get => _dealerCard5;
            set { _dealerCard5 = NewCard(value, nameof(DealerCard5)); }
        }

        string NewCard(string value, string propertyName)
        {
            Notify(propertyName);
            return value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
