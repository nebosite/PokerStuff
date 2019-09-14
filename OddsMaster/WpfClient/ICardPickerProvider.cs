using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    public interface ICardPickerProvider
    {
        CardPickerControl GetCardPicker();
    }
}
