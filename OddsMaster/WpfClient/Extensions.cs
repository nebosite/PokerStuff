using PokerParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    public static class OddsResultsExtensions
    {
        public static string GetExplanation(this OddsResults odds)
        {
            var output = new StringBuilder();
            output.AppendLine($"Win percentage: { (odds.WinRatio * 100.0).ToString(".0")}% ");
            output.AppendLine("\r\nHands performance:");
            var villianInfo = odds.VillianPerformance.Select(p => Tuple.Create(p.Key, p.Value)).OrderByDescending(t => t.Item2).ToArray();
            var playerInfo = odds.PlayerPerformance.Select(p => Tuple.Create(p.Key, p.Value)).OrderByDescending(t => t.Item2).ToArray();
            var formatter = new FixedFormatter();
            formatter.ColumnWidths.AddRange(new int[] { -8, 25, -8, 25 });
            output.AppendLine("Your Winning Hands                  Winning Opponent Hands");

            for (int i = 0; i < villianInfo.Length; i++)
            {
                output.AppendLine(formatter.Format(
                    $"{(playerInfo[i].Item2 * 100.0 * odds.WinRatio).ToString("0.")}%",
                    playerInfo[i].Item1.ToString(),
                    $"{(villianInfo[i].Item2 * 100.0).ToString("0.")}%",
                    villianInfo[i].Item1.ToString()
                ));
            }
            return output.ToString();
        }
    }
}
