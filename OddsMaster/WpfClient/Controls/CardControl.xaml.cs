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
        public CardModel ContextModel => DataContext as CardModel;

        public CardControl()
        {
            InitializeComponent();
            DataContextChanged += CardControl_DataContextChanged;
        }

        private void CardControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = this;
            var color = Brushes.Red;
            var card = ContextModel;
            
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

        private void HandleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ContextModel == null || !ContextModel.Available)
            {
                return;
            }

            var provider = WpfHelper.FindParentWithInterface(this, typeof(ICardClickProvider)) as ICardClickProvider;
            if (provider == null) return;

            provider.ClickedOnCard(this);
        }
    }
}
