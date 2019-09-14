using PokerParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    public interface ICardClickProvider
    {
        void ClickedOnCard(CardControl card);
    }
}
