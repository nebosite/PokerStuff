using OddsMaster;
using System.Windows;
using System.Windows.Controls;

namespace OddsMaster
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
        }
    }
}
