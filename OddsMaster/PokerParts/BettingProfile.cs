using System;
using System.Collections.Generic;
using System.Text;

namespace PokerParts
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Property bag to describe the betting profile of a table.
    /// </summary>
    //------------------------------------------------------------------------------------
    public class BettingProfile
    {
        public int Foldable { get; set; }
        public int Weak { get; set; }
        public int Regular { get; set; }
        public int Strong { get; set; }
        public List<Card[]> FoldablePairs { get; set; } = new List<Card[]>();
        public List<Card[]> WeakPairs { get; set; } = new List<Card[]>();
        public List<Card[]> RegularPairs { get; set; } = new List<Card[]>();
        public List<Card[]> StrongPairs { get; set; } = new List<Card[]>();
    }

}
