using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    class AppModel : INotifyPropertyChanged
    {
        public FlashGameModel FlashGame { get; set; } = new FlashGameModel();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void ResetFlashGame()
        {
            FlashGame.Reset();
        }

        internal void Recalculate()
        {
            FlashGame.Recalculate();
        }
    }
}
