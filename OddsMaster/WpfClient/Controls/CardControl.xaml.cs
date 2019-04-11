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
        public Card Card
        {
            get
            {
                return (Card)GetValue(CardProperty);
            }
            set
            {
                SetValue(CardProperty, value);
            }
        }

        public static readonly DependencyProperty CardProperty =
            DependencyProperty.Register("Card", typeof(Card), typeof(CardControl),new PropertyMetadata(cardPropertyChanged));

        private static void cardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {          
            var control = (CardControl)d;
            var color = Brushes.Red;
            var newCard = (Card)e.NewValue;
            
            if(newCard == null)
            {
                control.Visibility = Visibility.Hidden;
            }
            else
            {
                control.Visibility = Visibility.Visible;

                if (newCard.Suit == Suit.Clubs || newCard.Suit == Suit.Spades) color = Brushes.Black;
                control.CardRankLabel.Foreground = color;
                control.CardSuitLabel.Foreground = color;

                string suitText = "";
                switch(newCard.Suit)
                {
                    case Suit.Clubs: suitText = "§"; break;
                    case Suit.Hearts: suitText = "©"; break;
                    case Suit.Diamonds: suitText = "¨"; break;
                    case Suit.Spades: suitText = "ª"; break;

                }

                string rankText = "";
                switch (newCard.Rank)
                {
                    case Rank.Ace: rankText = "A"; break;
                    case Rank.King: rankText = "K"; break;
                    case Rank.Queen: rankText = "Q"; break;
                    case Rank.Jack: rankText = "J"; break;
                    case Rank._10: rankText = "10"; break;
                    case Rank._9: rankText = "9"; break;
                    case Rank._8: rankText = "8"; break;
                    case Rank._7: rankText = "7"; break;
                    case Rank._6: rankText = "6"; break;
                    case Rank._5: rankText = "5"; break;
                    case Rank._4: rankText = "4"; break;
                    case Rank._3: rankText = "3"; break;
                    case Rank._2: rankText = "2"; break;
                }
                control.CardSuitLabel.Content = suitText;
                control.CardRankLabel.Content = rankText;
            }

        }

        public CardControl()
        {
            InitializeComponent();
            Card = null;
            CardSuitLabel.Content = "";
            CardRankLabel.Content = "";
        }
    }
}
