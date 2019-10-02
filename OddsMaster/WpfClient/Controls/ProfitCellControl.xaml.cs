using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OddsMaster
{
    /// <summary>
    /// Interaction logic for ProfitCellControl.xaml
    /// </summary>
    public partial class ProfitCellControl : UserControl
    {
        ProfitTableItem ContextModel => DataContext as ProfitTableItem;
        public ProfitCellControl()
        {
            InitializeComponent();
        }

        private void HandleClick(object sender, MouseButtonEventArgs e)
        {
            ContextModel.ExplainMe();
        }
    }
}
