using Newtonsoft.Json;
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
    class AppModel : BaseModel
    {
        public ProfitModel Profit { get; set; } = new ProfitModel();
        public FlashGameModel FlashGame { get; set; } = new FlashGameModel();
        public TableGenModel TableGen { get; set; } = new TableGenModel();
        public static Dictionary<int, Dictionary<string, double>> PocketHandOdds { get; private set; }

        static AppModel()
        {
            var resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(n => n.Contains("PocketHandOdds")).FirstOrDefault();
            using (var oddsStream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)))
            {
                var oddsJson = oddsStream.ReadToEnd();
                PocketHandOdds = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, double>>>(oddsJson);
            }
        }
    }
}
