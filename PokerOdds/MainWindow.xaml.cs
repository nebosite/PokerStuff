using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        private void CalculateClick(object sender, RoutedEventArgs e)
        {
            var deck = new Deck();
            var hands = new Hand[9];

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                for (int j = 0; j < hands.Length; j++)
                {
                    hands[j] = new Hand();
                }

                deck.Reset();
                deck.Shuffle();
                for (int k = 0; k < 2; k++)
                {
                    for (int j = 0; j < hands.Length; j++)
                    {
                        hands[j].AddCard(deck.Draw());
                    }
                }

                var flop = new Card[3] { deck.Draw(), deck.Draw(), deck.Draw() };
                var turn = deck.Draw();
                var river = deck.Draw();

                for (int j = 0; j < hands.Length; j++)
                {
                    hands[j].AddCard(flop[0]);
                    hands[j].AddCard(flop[1]);
                    hands[j].AddCard(flop[2]);
                    hands[j].AddCard(turn);
                    hands[j].AddCard(river);
                }

                for (int j = 1; j < hands.Length; j++)
                {
                    hands[0].CompareTo(hands[j]);
                }
            }

            var elapsed = stopwatch.Elapsed;
            Debug.WriteLine("Elapsed Milliseconds: " + (elapsed.TotalSeconds * 1000).ToString(".00"));
        }
    }
}
