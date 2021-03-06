﻿using System;
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
using PokerParts;

namespace OddsMaster
{
    /// <summary>
    /// Interaction logic for ProfitControl.xaml
    /// </summary>
    public partial class ProfitControl : UserControl, ICardClickProvider
    {
        ProfitModel ContextModel => DataContext as ProfitModel;

        public ProfitControl()
        {
            InitializeComponent();
        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            ContextModel.Reset();
        }

        private void DealNextClick(object sender, RoutedEventArgs e)
        {
            ContextModel.DealNext();
        }

        public void ClickedOnCard(CardControl card)
        {
            TheCardPicker.PickCardFor(card);
        }
    }
}