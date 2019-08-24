using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddsMaster
{
    public class FixedFormatter
    {
        public List<int> ColumnWidths = new List<int>();
        public int Padding = 1;

        public string Format(params string[] columnValues)
        {
            var line = new StringBuilder();
            for (int i = 0; i < columnValues.Length; i++)
            {
                var width = 8;
                if (i < ColumnWidths.Count) width = ColumnWidths[i];
                var left = true;
                if (width < 0)
                {
                    left = false;
                    width = -width;
                }

                var text = columnValues[i];
                if (left)
                {
                    line.Append(text);
                    line.Append(new string(' ', width - text.Length));
                }
                else
                {
                    line.Append(new string(' ', width - text.Length));
                    line.Append(text);
                }
                line.Append(new string(' ', Padding));

            }

            return line.ToString();
        }
    }

}
