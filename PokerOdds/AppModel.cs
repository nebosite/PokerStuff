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
       

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
