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
    /// Interaction logic for FlashGameControl.xaml
    /// </summary>
    public partial class FlashGameControl : UserControl, ICardClickProvider
    {
        FlashGameModel TheModel => DataContext as FlashGameModel;

        public FlashGameControl()
        {
            InitializeComponent();
        }

        private void RedealClick(object sender, RoutedEventArgs e)
        {
            TheModel.Reset();
        }

        private void RecalcClick(object sender, RoutedEventArgs e)
        {
            TheModel.Recalculate();
        }

        private void ExplainClick(object sender, RoutedEventArgs e)
        {
            TheModel.ShowExplanation();
        }

        public void ClickedOnCard(CardControl card)
        {
            TheCardPicker.PickCardFor(card);
        }
    }
}
