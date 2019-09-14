using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OddsMaster
{
    public class DummyCard
    {
        public string RankText => "A";
        public string SuitText => "§";
        public bool Available => true;
    }

    public class DummyProfitItem
    {
        public string VisibleText => "-2.2";
        public double ProfitValue => -3;
    }
}
