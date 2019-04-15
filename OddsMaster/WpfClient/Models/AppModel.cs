using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    class AppModel : BaseModel
    {
        public FlashGameModel FlashGame { get; set; } = new FlashGameModel();
        public TableGenModel TableGen { get; set; } = new TableGenModel();


    }
}
