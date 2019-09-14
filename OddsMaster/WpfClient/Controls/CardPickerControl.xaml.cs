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

namespace OddsMaster
{
    /// <summary>
    /// Interaction logic for CardPickerControl.xaml
    /// </summary>
    public partial class CardPickerControl : UserControl
    {
        public CardPickerControl()
        {
            InitializeComponent();
        }

        internal void PickCardFor(CardControl cardControl)
        {
            this.Visibility = Visibility.Visible;
        }

        private void HandleOutsideMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
