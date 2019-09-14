using Newtonsoft.Json;
using PokerParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    //------------------------------------------------------------------------------------
    /// <summary>
    /// General model for displaying a card
    /// </summary>
    //------------------------------------------------------------------------------------
    public class CardModel : BaseModel
    {
        private Card _card;
        public Card Card
        {
            get => _card;
            set
            {
                _card = value;
                NotifyAllPropertiesChanged();
            }
        }

        private bool _available;
        public bool Available
        {
            get => _available;
            set
            {
                _available = value;
                NotifyPropertyChanged(nameof(Available));
            }
        }

        public string SuitText => _card.SuitText;
        public string RankText => _card.RankText;
        public Suit Suit => _card.Suit;

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ctor
        /// </summary>
        //------------------------------------------------------------------------------------
        public CardModel(Card card, bool available = true)
        {
            Card = card;
            Available = available;
        }

        //------------------------------------------------------------------------------------
        /// <summary>
        /// ToString
        /// </summary>
        //------------------------------------------------------------------------------------
        public override string ToString()
        {
            return Card.ToString();
        }
    }
}
