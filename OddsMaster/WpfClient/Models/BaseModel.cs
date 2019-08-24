using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    public class BaseModel  : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyAllPropertiesChanged()
        {
            foreach(var property in GetType().GetProperties())
            {
                NotifyPropertyChanged(property.Name);
            }
        }
    }
}
