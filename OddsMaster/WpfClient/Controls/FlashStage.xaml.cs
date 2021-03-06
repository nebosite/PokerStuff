﻿using OddsMaster;
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

            Model.SelectStrength(int.Parse(button.Tag.ToString()));
        }

        private void Button_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {

        }

        private void DealNextClick(object sender, RoutedEventArgs e)
        {
            Model.DealNext();
        }
    }
}
