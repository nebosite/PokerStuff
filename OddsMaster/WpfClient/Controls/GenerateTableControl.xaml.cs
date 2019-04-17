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
    /// Interaction logic for GenerateTableControl.xaml
    /// </summary>
    public partial class GenerateTableControl : UserControl
    {       
        TableGenModel TheModel => DataContext as TableGenModel;
        public GenerateTableControl()
        {
            InitializeComponent();
        }

        private void GenerateTableClick(object sender, RoutedEventArgs e)
        {
            TheModel.Generate();
        }

        private void FlashPlayerCountChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}