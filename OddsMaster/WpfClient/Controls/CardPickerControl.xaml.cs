using PokerParts;
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
    /// Interaction logic for CardPickerControl.xaml
    /// </summary>
    public partial class CardPickerControl : UserControl, ICardClickProvider
    {
        CardControl _targetCard;
        BaseGameAnalysisModel ContextModel => DataContext as BaseGameAnalysisModel;

        public CardPickerControl()
        {
            InitializeComponent();
        }

        public void ClickedOnCard(CardControl card)
        {
            ContextModel.ReplaceCard(_targetCard.ContextModel, card.ContextModel);
            _targetCard.CardBorder.BorderBrush = Brushes.Gray;
            this.Visibility = Visibility.Collapsed;
        }

        internal void PickCardFor(CardControl cardControl)
        {
            this.Visibility = Visibility.Visible;
            _targetCard = cardControl;
            _targetCard.CardBorder.BorderBrush = Brushes.Black;
        }

        private void HandleOutsideMouseUp(object sender, MouseButtonEventArgs e)
        {
            _targetCard.CardBorder.BorderBrush = Brushes.Gray;
            this.Visibility = Visibility.Collapsed;
        }
    }
}
