using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// Handles generating tables of poker odds
    /// </summary>
    //------------------------------------------------------------------------------------
    class TableGenModel : BaseModel
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

        string _genOutput = "Click 'Generate' to output table values";
        public string GenOutput
        {
            get => _genOutput;
            set
            {
                _genOutput = value;
                Notify(nameof(GenOutput));
            }
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public TableGenModel()
        {
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// Generate some data
        /// </summary>
        //------------------------------------------------------------------------------------
        public void Generate()
        {
        }
    }
}
