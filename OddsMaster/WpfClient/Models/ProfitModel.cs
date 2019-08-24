using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Windows.Media;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Handles examining the profitability of a hand
    /// </summary>
    //------------------------------------------------------------------------------------
    class ProfitModel : BaseGameAnalysisModel
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public ProfitModel()
        {
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Reset this model and draw some cards
        /// </summary>
        //------------------------------------------------------------------------------------
        internal void Reset()
        {
            base.Reset();
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Deal the next cards
        /// </summary>
        //------------------------------------------------------------------------------------
        public void DealNext()
        {
            base.DealNext();
        }
    }
}
