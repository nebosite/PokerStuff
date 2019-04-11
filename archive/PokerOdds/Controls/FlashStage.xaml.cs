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

namespace PokerOdds
{
    /// <summary>
    /// Interaction logic for FlashStage.xaml
    /// </summary>
    public partial class FlashStage : UserControl
    {
        public FlashStage()
        {
            InitializeComponent();
        }

        FlashGameModel Model => DataContext as FlashGameModel;

        private void StrengthClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Model.UserSelectedStrength(button.Content.ToString());
        }
    }
}
