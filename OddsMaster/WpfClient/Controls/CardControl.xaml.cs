using PokerParts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OddsMaster
{
    /// <summary>
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        public CardControl()
        {
            InitializeComponent();
            DataContextChanged += CardControl_DataContextChanged;
        }

        private void CardControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = this;
            var color = Brushes.Red;
            var card = DataContext as Card;
            
            if(card == null)
            {
                control.Visibility = Visibility.Hidden;
            }
            else
            {
                control.Visibility = Visibility.Visible;

                if (card.Suit == Suit.Clubs || card.Suit == Suit.Spades) color = Brushes.Black;
                control.CardRankLabel.Foreground = color;
                control.CardSuitLabel.Foreground = color;
            }
        }
    }
}
